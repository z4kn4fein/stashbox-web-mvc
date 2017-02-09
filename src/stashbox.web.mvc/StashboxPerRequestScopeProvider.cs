using System.Web;
using Stashbox.Infrastructure;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents a per request scope provider interface.
    /// </summary>
    public interface IPerRequestScopeProvider
    {
        /// <summary>
        /// Gets or creates a per request scope.
        /// </summary>
        /// <returns>The existing or a new scope.</returns>
        IStashboxContainer GetOrCreateScope();
    }

    /// <summary>
    /// The <see cref="IPerRequestScopeProvider"/> implementation which uses <see cref="IStashboxContainer"/> as scope manager.
    /// </summary>
    public class StashboxPerRequestScopeProvider : IPerRequestScopeProvider
    {
        private readonly IStashboxContainer rootScope;
        private const string ScopeKey = "requestScope";

        /// <summary>
        /// Constructs a <see cref="StashboxPerRequestScopeProvider"/>.
        /// </summary>
        /// <param name="rootScope">The root <see cref="IStashboxContainer"/> instance.</param>
        public StashboxPerRequestScopeProvider(IStashboxContainer rootScope)
        {
            this.rootScope = rootScope;
        }

        /// <inheritdoc />
        public IStashboxContainer GetOrCreateScope()
        {
            var scope = HttpContext.Current.Items[ScopeKey] as IStashboxContainer;

            if (scope == null)
                HttpContext.Current.Items[ScopeKey] = scope = this.rootScope.BeginScope();

            return scope;
        }

        /// <summary>
        /// Closes the current per request scope.
        /// </summary>
        public static void TerminateScope()
        {
            var scope = HttpContext.Current.Items[ScopeKey] as IStashboxContainer;
            scope?.Dispose();
        }
    }
}
