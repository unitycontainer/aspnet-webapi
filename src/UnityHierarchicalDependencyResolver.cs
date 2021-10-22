﻿using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Unity.AspNet.WebApi
{
    /// <summary>
    /// An implementation of <see cref="IDependencyResolver"/> that wraps a Unity container and creates a new child container
    /// when <see cref="BeginScope"/> is invoked.
    /// </summary>
    /// <remarks>
    /// Because each scope creates a new child Unity container, you can benefit from using the <see cref="Unity.WithLifetime.HierarchicalLifetimeManager"/>
    /// lifetime manager.
    /// </remarks>
    public sealed class UnityHierarchicalDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityHierarchicalDependencyResolver"/> class for a container.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> to wrap with the <see cref="IDependencyResolver"/>
        /// interface implementation.</param>
        public UnityHierarchicalDependencyResolver(IUnityContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <summary>
        /// Starts a resolution scope by creating a new child Unity container.
        /// </summary>
        /// <returns>The dependency scope.</returns>
        public IDependencyScope BeginScope()
        {
            return new UnityHierarchicalDependencyScope(_container);
        }

        /// <summary>
        /// Disposes the wrapped <see cref="IUnityContainer"/>.
        /// </summary>
        public void Dispose()
        {
            _container.Dispose();
        }

        /// <summary>
        /// Resolves an instance of the default requested type from the container.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the object to get from the container.</param>
        /// <returns>The retrieved object.</returns>
        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>The requested services.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        private sealed class UnityHierarchicalDependencyScope : IDependencyScope
        {
            private IUnityContainer container;

            public UnityHierarchicalDependencyScope(IUnityContainer parentContainer)
            {
                container = parentContainer.CreateChildContainer();
            }

            public object GetService(Type serviceType)
            {
                return container.Resolve(serviceType);
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return container.ResolveAll(serviceType);
            }

            public void Dispose()
            {
                container.Dispose();
            }
        }
    }
}
