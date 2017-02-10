using System;
using System.Collections.Generic;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox dependency resolver.
    /// </summary>
    public class StashboxDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return StashboxPerRequestScopeProvider.GetOrCreateScope().CanResolve(serviceType) ?
                StashboxPerRequestScopeProvider.GetOrCreateScope().Resolve(serviceType) : null;
        }

        /// <inheritdoc />
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return StashboxPerRequestScopeProvider.GetOrCreateScope().CanResolve(serviceType) ?
                StashboxPerRequestScopeProvider.GetOrCreateScope().ResolveAll(serviceType) : new List<object>();
        }
    }
}
