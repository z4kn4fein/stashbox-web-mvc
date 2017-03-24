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
        private volatile Expression expression;
        private readonly object syncObject = new object();
        private readonly string scopeId;

        /// <summary>
        /// Constructs a <see cref="PerRequestLifetime"/>.
        /// </summary>
        public PerRequestLifetime() : this(Guid.NewGuid().ToString())
        { }

        private PerRequestLifetime(string scopeId)
        {
            this.scopeId = scopeId;
        }

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;
                var expr = base.GetExpression(containerContext, objectBuilder, resolutionInfo, resolveType);
                if (expr == null)
                    return null;

                var factory = expr.CompileDelegate(Stashbox.Constants.ScopeExpression);

                var method = Constants.GetScopedValueMethod.MakeGenericMethod(resolveType);

                this.expression = Expression.Call(method,
                    Stashbox.Constants.ScopeExpression,
                    Expression.Constant(factory),
                    Expression.Constant(this.scopeId));
            }

            return this.expression;
        }

        private static TValue CollectScopedInstance<TValue>(IResolutionScope scope, Func<IResolutionScope, object> factory, string scopeId)
            where TValue : class
        {
            if (HttpContext.Current == null)
                return null;

            if (HttpContext.Current.Items[scopeId] != null)
                return HttpContext.Current.Items[scopeId] as TValue;

            TValue instance;
            if (HttpContext.Current.Items[StashboxPerRequestScopeProvider.ScopeKey] is IResolutionScope requestScope)
            {
                instance = factory(requestScope) as TValue;
                HttpContext.Current.Items[scopeId] = instance;
            }
            else
                instance = factory(scope) as TValue;

            return instance;
        }

        /// <inheritdoc />
        public override ILifetime Create() => new PerRequestLifetime(this.scopeId);
    }
}
