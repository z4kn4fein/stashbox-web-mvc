using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Stashbox.Utils;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox filter attribute filter provider.
    /// </summary>
    public class StashboxFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        private readonly Infrastructure.IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxFilterAttributeFilterProvider"/>
        /// </summary>
        /// <param name="dependencyResolver">The stashbox container instance.</param>
        public StashboxFilterAttributeFilterProvider(Infrastructure.IDependencyResolver dependencyResolver)
        {
            Shield.EnsureNotNull(dependencyResolver, nameof(dependencyResolver));

            this.dependencyResolver = dependencyResolver;
        }

        /// <inheritdoc />
        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor).ToArray();
            foreach (var filterAttribute in attributes)
                this.dependencyResolver.BuildUp(filterAttribute);

            return attributes;
        }

        /// <inheritdoc />
        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor).ToArray();
            foreach (var filterAttribute in attributes)
                this.dependencyResolver.BuildUp(filterAttribute);

            return attributes;
        }
    }
}
