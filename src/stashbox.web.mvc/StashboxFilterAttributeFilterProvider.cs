using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Stashbox.Infrastructure;
using Stashbox.Utils;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox filter attribute filter provider.
    /// </summary>
    public class StashboxFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        private readonly IStashboxContainer stashboxContainer;

        /// <summary>
        /// Constructs a <see cref="StashboxFilterAttributeFilterProvider"/>
        /// </summary>
        /// <param name="stashboxContainer">The stashbox container instance.</param>
        public StashboxFilterAttributeFilterProvider(IStashboxContainer stashboxContainer)
        {
            Shield.EnsureNotNull(stashboxContainer, nameof(stashboxContainer));

            this.stashboxContainer = stashboxContainer;
        }

        /// <inheritdoc />
        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor).ToArray();
            foreach (var filterAttribute in attributes)
                this.stashboxContainer.BuildUp(filterAttribute);

            return attributes;
        }

        /// <inheritdoc />
        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor).ToArray();
            foreach (var filterAttribute in attributes)
                this.stashboxContainer.BuildUp(filterAttribute);

            return attributes;
        }
    }
}
