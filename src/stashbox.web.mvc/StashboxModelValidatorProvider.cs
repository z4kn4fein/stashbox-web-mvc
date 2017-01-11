using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ronin.Common;
using Stashbox.Infrastructure;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox model validator provider.
    /// </summary>
    public class StashboxModelValidatorProvider : ModelValidatorProvider
    {
        private readonly IStashboxContainer stashboxContainer;
        private readonly IEnumerable<ModelValidatorProvider> modelValidatorProviders;

        /// <summary>
        /// Constructs a <see cref="StashboxModelValidatorProvider"/>
        /// </summary>
        /// <param name="stashboxContainer">The stashbox container instance.</param>
        /// <param name="modelValidatorProviders">The collection of the existing model validator providers.</param>
        public StashboxModelValidatorProvider(IStashboxContainer stashboxContainer, IEnumerable<ModelValidatorProvider> modelValidatorProviders)
        {
            Shield.EnsureNotNull(stashboxContainer, nameof(stashboxContainer));
            Shield.EnsureNotNull(modelValidatorProviders, nameof(modelValidatorProviders));

            this.stashboxContainer = stashboxContainer;
            this.modelValidatorProviders = modelValidatorProviders;
        }
        
        /// <inheritdoc />
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var validators = this.modelValidatorProviders.SelectMany(provider => provider.GetValidators(metadata, context)).ToList();
            foreach (var modelValidator in validators)
                this.stashboxContainer.BuildUp(modelValidator);

            return validators;
        }
    }
}
