using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Lifetime;
using System;
using System.Linq.Expressions;
using System.Web;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents a per request lifetime.
    /// </summary>
    public class PerRequestLifetime : LifetimeBase
    {
        private readonly object sync = new object();
        private readonly int scopedId;

        /// <summary>
        /// Constructs a <see cref="PerRequestLifetime"/>.
        /// </summary>
        public PerRequestLifetime()
        {
            this.scopedId = Guid.NewGuid().GetHashCode();
        }

        private PerRequestLifetime(int scopedId)
        {
            this.scopedId = scopedId;
        }

        /// <inheritdoc />
        public override bool IsScoped => true;

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            var call = Expression.Call(Expression.Constant(this), "CollectScopedInstance", null,
                Expression.Constant(containerContext),
                Expression.Constant(objectBuilder),
                Expression.Constant(resolutionInfo),
                Expression.Constant(resolveType));
            return Expression.Convert(call, resolveType.Type);
        }

        private object CollectScopedInstance(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            lock (this.sync)
            {
                if (HttpContext.Current == null)
                    return null;

                if (HttpContext.Current.Items[this.scopedId] != null)
                    return HttpContext.Current.Items[this.scopedId];

                var scope = HttpContext.Current.Items[StashboxPerRequestScopeProvider.ScopeKey] as IStashboxContainer;
                object instance = null;
                if (containerContext.Container == scope)
                {
                    var expr = base.GetExpression(containerContext, objectBuilder, resolutionInfo, resolveType);
                    instance = Expression.Lambda<Func<object>>(expr).Compile()();
                }
                else if (scope != null)
                    instance = scope.ActivationContext.Activate(resolutionInfo, resolveType);

                HttpContext.Current.Items[this.scopedId] = instance;

                return instance;
            }
        }

        /// <inheritdoc />
        public override ILifetime Create()
        {
            return new PerRequestLifetime(this.scopedId);
        }

        /// <inheritdoc />
        public override void CleanUp()
        {
            lock (this.sync)
            {
                if (HttpContext.Current == null)
                    return;

                if (HttpContext.Current.Items[this.scopedId] != null)
                {
                    var instance = HttpContext.Current.Items[this.scopedId] as IDisposable;
                    if (instance != null)
                    {
                        instance.Dispose();
                        HttpContext.Current.Items[this.scopedId] = null;
                    }
                }
            }
        }
    }
}
