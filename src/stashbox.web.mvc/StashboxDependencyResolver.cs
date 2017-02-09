using Stashbox.Utils;
using System;
using System.Collections.Generic;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox dependency resolver.
    /// </summary>
    public class StashboxDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        private readonly IPerRequestScopeProvider perRequestScopeProvider;

        /// <summary>
        /// Constructs a <see cref="StashboxDependencyResolver"/>
        /// </summary>
        /// <param name="perRequestScopeProvider">The per request scope provider.</param>
        public StashboxDependencyResolver(IPerRequestScopeProvider perRequestScopeProvider)
        {
            Shield.EnsureNotNull(perRequestScopeProvider, nameof(perRequestScopeProvider));

            this.perRequestScopeProvider = perRequestScopeProvider;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return this.perRequestScopeProvider.GetOrCreateScope().CanResolve(serviceType) ? 
                this.perRequestScopeProvider.GetOrCreateScope().Resolve(serviceType) : null;
        }

        /// <inheritdoc />
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.perRequestScopeProvider.GetOrCreateScope().CanResolve(serviceType) ? 
                this.perRequestScopeProvider.GetOrCreateScope().ResolveAll(serviceType) : new List<object>();
        }
    }
}
