using Stashbox.BuildUp;
using Stashbox.Lifetime;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;
using System.Web;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents a per request lifetime.
    /// </summary>
    public class PerRequestLifetime : ScopedLifetimeBase
    {
        private volatile Expression expression;
        private readonly object syncObject = new object();

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;
                var factory = base.GetFactoryExpression(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
                if (factory == null)
                    return null;

                return this.expression = Constants.GetScopedValueMethod.MakeGenericMethod(resolveType)
                    .InvokeMethod(resolutionContext.CurrentScopeParameter, factory, base.ScopeId.AsConstant());
            }
        }

        private static TValue CollectScopedInstance<TValue>(IResolutionScope scope, Func<IResolutionScope, object> factory, object scopeId)
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
        public override ILifetime Create() => new PerRequestLifetime();
    }
}
