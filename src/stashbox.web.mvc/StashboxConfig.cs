using System;
using System.Linq;
using System.Web.Mvc;
using Stashbox.Infrastructure;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox configuration for ASP.NET MVC.
    /// </summary>
    public class StashboxConfig
    {
        private static readonly Lazy<StashboxConfig> instance = new Lazy<StashboxConfig>();
        private static readonly Lazy<IStashboxContainer> stashboxContainer = new Lazy<IStashboxContainer>(() => new StashboxContainer());

        /// <summary>
        /// Singleton instance of the <see cref="StashboxConfig"/>.
        /// </summary>
        public static StashboxConfig Instance => instance.Value;

        /// <summary>
        /// Singleton instance of the <see cref="StashboxContainer"/>.
        /// </summary>
        public static IStashboxContainer Container => stashboxContainer.Value;

        /// <summary>
        /// Registers all the <see cref="StashboxConfig"/> features.
        /// </summary>
        public void UseStashbox()
        {
            this.UseStashboxResolver()
                .UseStashboxFilterAttributeProvider()
                .UseStashboxDataAnnotationModelValidatorProvider();
        }

        /// <summary>
        /// Sets the <see cref="StashboxContainer"/> as the default dependency resolver. Calls the <see cref="RegisterComponents"/> virtual method.
        /// </summary>
        /// <returns>The <see cref="StashboxConfig"/> instance.</returns>
        public StashboxConfig UseStashboxResolver()
        {
            DependencyResolver.SetResolver(new StashboxDependencyResolver(Container));
            RegisterComponents(Container);
            return this;
        }

        /// <summary>
        /// Inherited members should override this method, where they can customize the <see cref="StashboxContainer"/> instance and register their services.
        /// </summary>
        /// <param name="container">The <see cref="IStashboxContainer"/> instance.</param>
        protected virtual void RegisterComponents(IStashboxContainer container)
        { }

        /// <summary>
        /// Replaces the default <see cref="FilterAttributeFilterProvider"/> with the <see cref="StashboxFilterAttributeFilterProvider"/>.
        /// </summary>
        /// <returns>The <see cref="StashboxConfig"/> instance.</returns>
        public StashboxConfig UseStashboxFilterAttributeProvider()
        {
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().Single());
            FilterProviders.Providers.Add(new StashboxFilterAttributeFilterProvider(Container));
            return this;
        }

        /// <summary>
        /// Replaces the default <see cref="DataAnnotationsModelValidatorProvider"/> with the <see cref="StashboxDataAnnotationsModelValidatorProvider"/>.
        /// </summary>
        /// <returns>The <see cref="StashboxConfig"/> instance.</returns>
        public StashboxConfig UseStashboxDataAnnotationModelValidatorProvider()
        {
            ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.OfType<DataAnnotationsModelValidatorProvider>().Single());
            ModelValidatorProviders.Providers.Add(new StashboxDataAnnotationsModelValidatorProvider(Container));
            return this;
        }
    }
}
