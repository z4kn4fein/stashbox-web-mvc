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
        /// Sets the <see cref="StashboxContainer"/> as the default dependency resolver. Calls the <see cref="RegisterComponents"/> virtual method.
        /// </summary>
        public void UseStashbox()
        {
            DependencyResolver.SetResolver(new StashboxDependencyResolver(Container));
            this.RegisterStashboxComponents(Container);
            this.RemoveDefaultProviders();
            this.RegisterComponents(Container);
        }
        
        /// <summary>
        /// Inherited members should override this method, where they can customize the <see cref="StashboxContainer"/> instance and register their services.
        /// </summary>
        /// <param name="container">The <see cref="IStashboxContainer"/> instance.</param>
        protected virtual void RegisterComponents(IStashboxContainer container)
        { }
        
        private void RegisterStashboxComponents(IDependencyRegistrator container)
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

        private void RemoveDefaultProviders()
        {
            FilterProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Clear();
        }
    }
}
