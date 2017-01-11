using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Ronin.Common;
using Stashbox.Infrastructure;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox data annotations model validator provider.
    /// </summary>
    public class StashboxDataAnnotationsModelValidatorProvider : DataAnnotationsModelValidatorProvider
    {
        private readonly IStashboxContainer stashboxContainer;

        private readonly MethodInfo attributeGetter;

        /// <summary>
        /// Constructs a <see cref="StashboxDataAnnotationsModelValidatorProvider"/>
        /// </summary>
        /// <param name="stashboxContainer">The stashbox container instance.</param>
        public StashboxDataAnnotationsModelValidatorProvider(IStashboxContainer stashboxContainer)
        {
            Shield.EnsureNotNull(stashboxContainer, nameof(stashboxContainer));

            this.stashboxContainer = stashboxContainer;
            this.attributeGetter = typeof(DataAnnotationsModelValidator).GetMethod("get_Attribute",
                BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        }

        /// <inheritdoc />
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
        {
            var validators = base.GetValidators(metadata, context, attributes).ToArray();
            foreach (var modelValidator in validators)
            {
                var attribute = this.attributeGetter.Invoke(modelValidator, new object[0]);
                this.stashboxContainer.BuildUp(attribute);
            }

            return validators;
        }
    }
}
