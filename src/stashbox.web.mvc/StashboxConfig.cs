using System;
using System.Linq;
using System.Web.Mvc;
using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox configuration for ASP.NET MVC.
    /// </summary>
    public static class StashboxConfig
    {
        private static readonly Lazy<IStashboxContainer> stashboxContainer = new Lazy<IStashboxContainer>(() => new StashboxContainer());
        
        /// <summary>
        /// Singleton instance of the <see cref="StashboxContainer"/>.
        /// </summary>
        public static IStashboxContainer Container => stashboxContainer.Value;

        /// <summary>
        /// Sets the <see cref="StashboxContainer"/> as the default dependency resolver and sets custom <see cref="IFilterProvider"/> and <see cref="ModelValidatorProvider"/>.
        /// </summary>
        public static void RegisterStashbox(Action<IStashboxContainer> configureAction)
        {
            DependencyResolver.SetResolver(new StashboxDependencyResolver(Container));
            RegisterStashboxComponents(Container);
            RemoveDefaultProviders();
            configureAction(Container);
        }
        
        private static void RegisterStashboxComponents(IDependencyRegistrator container)
        {
            container.RegisterInstance<IStashboxContainer>(container);

            container.RegisterType<ModelValidatorProvider, StashboxDataAnnotationsModelValidatorProvider>();
            container.PrepareType<ModelValidatorProvider, StashboxModelValidatorProvider>()
                .WithInjectionParameters(new InjectionParameter
                {
                    Name = "modelValidatorProviders",
                    Value = ModelValidatorProviders.Providers.Where(provider => !(provider is DataAnnotationsModelValidatorProvider)).ToArray()
                }).Register();

            container.RegisterType<IFilterProvider, StashboxFilterAttributeFilterProvider>();
            container.PrepareType<IFilterProvider, StashboxFilterProvider>()
                .WithInjectionParameters(new InjectionParameter
                {
                    Name = "filterProviders",
                    Value = FilterProviders.Providers.Where(provider => !(provider is FilterAttributeFilterProvider)).ToArray()
                }).Register();
        }

        private static void RemoveDefaultProviders()
        {
            FilterProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Clear();
        }
    }
}
