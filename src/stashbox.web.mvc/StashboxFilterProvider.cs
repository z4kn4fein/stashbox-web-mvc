using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ronin.Common;
using Stashbox.Infrastructure;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox filter provider.
    /// </summary>
    public class StashboxFilterProvider : IFilterProvider
    {
        private readonly IStashboxContainer stashboxContainer;
        private readonly IEnumerable<IFilterProvider> filterProviders;

        /// <summary>
        /// Constructs a <see cref="StashboxFilterProvider"/>
        /// </summary>
        /// <param name="stashboxContainer">The stashbox container instance.</param>
        /// <param name="filterProviders">The collection of the existing filter providers.</param>
        public StashboxFilterProvider(IStashboxContainer stashboxContainer, IEnumerable<IFilterProvider> filterProviders)
        {
            Shield.EnsureNotNull(stashboxContainer, nameof(stashboxContainer));
            Shield.EnsureNotNull(filterProviders, nameof(filterProviders));

            this.stashboxContainer = stashboxContainer;
            this.filterProviders = filterProviders;
        }

        /// <inheritdoc />
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = this.filterProviders.SelectMany(provider => provider.GetFilters(controllerContext, actionDescriptor)).ToArray();
            foreach (var filter in filters)
                this.stashboxContainer.BuildUp(filter.Instance);

            return filters;
        }
    }
}
