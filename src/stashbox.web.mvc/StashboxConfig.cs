using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;
using Stashbox.Lifetime;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox configuration for ASP.NET MVC.
    /// </summary>
    public static class StashboxConfig
    {
        /// <summary>
        /// Sets the <see cref="StashboxContainer"/> as the default dependency resolver and sets custom <see cref="IFilterProvider"/> and <see cref="ModelValidatorProvider"/>.
        /// </summary>
        public static void RegisterStashbox(Action<IStashboxContainer> configureAction)
        {
            var container = new StashboxContainer(config => config
                .WithCircularDependencyTracking()
                .WithDisposableTransientTracking());

            DependencyResolver.SetResolver(new StashboxDependencyResolver(new StashboxPerRequestScopeProvider(container)));
            RegisterComponents(container);
            RemoveDefaultProviders();
            configureAction(container);
        }

        /// <summary>
        /// Sets the <see cref="StashboxContainer"/> as the default dependency resolver and sets custom <see cref="IFilterProvider"/> and <see cref="ModelValidatorProvider"/>.
        /// </summary>
        public static void RegisterStashbox(IStashboxContainer container)
        {
            DependencyResolver.SetResolver(new StashboxDependencyResolver(new StashboxPerRequestScopeProvider(container)));
            RegisterComponents(container);
            RemoveDefaultProviders();
        }

        private static void RegisterComponents(IStashboxContainer container)
        {
            container.RegisterInstance(container);
            container.RegisterType<ModelValidatorProvider, StashboxDataAnnotationsModelValidatorProvider>();
            container.RegisterType<ModelValidatorProvider, StashboxModelValidatorProvider>(context =>
                context.WithInjectionParameters(new InjectionParameter
                {
                    Name = "modelValidatorProviders",
                    Value = ModelValidatorProviders.Providers.Where(provider => !(provider is DataAnnotationsModelValidatorProvider)).ToArray()
                }));

            container.RegisterType<IFilterProvider, StashboxFilterAttributeFilterProvider>();
            container.RegisterType<IFilterProvider, StashboxFilterProvider>(context =>
                context.WithInjectionParameters(new InjectionParameter
                {
                    Name = "filterProviders",
                    Value = FilterProviders.Providers.Where(provider => !(provider is FilterAttributeFilterProvider)).ToArray()
                }));

            RegisterControllers(container);
        }

        private static void RegisterControllers(IDependencyRegistrator container)
        {
            var controllers = BuildManager.GetReferencedAssemblies().OfType<Assembly>()
                .Where(assembly => !assembly.IsDynamic && !assembly.GlobalAssemblyCache);

            container.RegisterAssemblies(controllers, type => typeof(IController).IsAssignableFrom(type),
                context => context.WithLifetime(new ScopedLifetime()));
        }

        private static void RemoveDefaultProviders()
        {
            FilterProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Clear();
        }
    }
}
