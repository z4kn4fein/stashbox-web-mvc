﻿using Stashbox.Infrastructure;
using Stashbox.Utils;
using System;
using System.Collections.Generic;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox dependency resolver.
    /// </summary>
    public class StashboxDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        private readonly IStashboxContainer stashboxContainer;

        /// <summary>
        /// Constructs a <see cref="StashboxDependencyResolver"/>
        /// </summary>
        /// <param name="stashboxContainer">The stashbox container instance.</param>
        public StashboxDependencyResolver(IStashboxContainer stashboxContainer)
        {
            Shield.EnsureNotNull(stashboxContainer, nameof(stashboxContainer));

            this.stashboxContainer = stashboxContainer;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return this.stashboxContainer.CanResolve(serviceType) ? this.stashboxContainer.Resolve(serviceType) : null;
        }

        /// <inheritdoc />
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.stashboxContainer.CanResolve(serviceType) ? this.stashboxContainer.ResolveAll(serviceType) : new List<object>();
        }
    }
}
