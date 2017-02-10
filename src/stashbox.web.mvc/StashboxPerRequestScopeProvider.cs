using Stashbox.Infrastructure;
using System;
using System.Web;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents a per request scope provider using the <see cref="StashboxContainer"/>.
    /// </summary>
    public class StashboxPerRequestScopeProvider
    {
        private const string ScopeKey = "requestScope";

        private static readonly Lazy<IStashboxContainer> stashboxContainer = new Lazy<IStashboxContainer>(() => new StashboxContainer(config =>
            config.WithCircularDependencyTracking()
            .WithDisposableTransientTracking()
            .WithParentContainerResolution()));

        /// <summary>
        /// Singleton instance of the <see cref="StashboxContainer"/>.
        /// </summary>
        public static IStashboxContainer Container => stashboxContainer.Value;

        /// <summary>
        /// Gets or creates a scope.
        /// </summary>
        /// <returns></returns>
        public static IStashboxContainer GetOrCreateScope()
        {
            var scope = HttpContext.Current?.Items[ScopeKey] as IStashboxContainer;

            if (scope == null && HttpContext.Current != null)
                HttpContext.Current.Items[ScopeKey] = scope = Container.BeginScope();

            return scope;
        }

        /// <summary>
        /// Closes the current per request scope.
        /// </summary>
        public static void TerminateScope()
        {
            var scope = HttpContext.Current?.Items[ScopeKey] as IStashboxContainer;
            scope?.Dispose();
        }
    }
}
