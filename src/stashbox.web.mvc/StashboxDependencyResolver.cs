using System;
using System.Collections.Generic;
using Stashbox.Infrastructure;

namespace Stashbox.Web.Mvc
{
    public class StashboxDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        private readonly IStashboxContainer stashboxContainer;

        public StashboxDependencyResolver(IStashboxContainer stashboxContainer)
        {
            this.stashboxContainer = stashboxContainer;
        }

        public object GetService(Type serviceType)
        {
            return this.stashboxContainer.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.stashboxContainer.ResolveAll(serviceType);
        }
    }
}
