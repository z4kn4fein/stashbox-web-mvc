using Stashbox.Infrastructure;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Web;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox dependency resolver.
    /// </summary>
    public class StashboxDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        private const string ScopeKey = "requestScope";
        private readonly IStashboxContainer stashboxContainer;

        /// <summary>
        /// Constructs a <see cref="StashboxDependencyResolver"/>
        /// </summary>
        /// <param name="stashboxContainer">The stashbox container instance.</param>
        public StashboxDependencyResolver(IStashboxContainer stashboxContainer)
        {
            Shield.EnsureNotNull(stashboxContainer, nameof(stashboxContainer));

            this.stashboxContainer = stashboxContainer;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return this.GetScope().CanResolve(serviceType) ? this.GetScope().Resolve(serviceType) : null;
        }

        /// <inheritdoc />
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.GetScope().CanResolve(serviceType) ? this.GetScope().ResolveAll(serviceType) : new List<object>();
        }

        /// <summary>
        /// Closes the current per request scope.
        /// </summary>
        public static void TerminateScope()
        {
            var scope = HttpContext.Current.Items[ScopeKey] as IStashboxContainer;
            scope?.Dispose();
        }

        private IStashboxContainer GetScope()
        {
                var scope = HttpContext.Current.Items[ScopeKey] as IStashboxContainer;

                if (scope == null)
                    HttpContext.Current.Items[ScopeKey] = scope = this.stashboxContainer.BeginScope();

                return scope;
        }
    }
}
