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
            return Expression.Convert(call, resolveType.Type); ;
        }

        private object CollectScopedInstance(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            lock (this.sync)
            {
                if (HttpContext.Current != null && HttpContext.Current.Items[scopedId] != null)
                    return HttpContext.Current.Items[scopedId];

                var scope = StashboxPerRequestScopeProvider.GetOrCreateScope();
                object instance = null;
                if (containerContext.Container == scope)
                {
                    var expr = base.GetExpression(containerContext, objectBuilder, resolutionInfo, resolveType);
                    instance = Expression.Lambda<Func<object>>(expr).Compile()();
                }
                else if (scope != null)
                    instance = scope.ActivationContext.Activate(resolutionInfo, resolveType);

                if (HttpContext.Current != null)
                    HttpContext.Current.Items[scopedId] = instance;

                return instance;
            }
        }

        /// <inheritdoc />
        public override ILifetime Create()
        {
            return new PerRequestLifetime(this.scopedId);
        }
    }
}
