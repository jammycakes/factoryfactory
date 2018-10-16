using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Impl;
using Nooshka.Resolution;

namespace Nooshka
{
    /// <summary>
    ///  The core class which is responsible for resolution and management of
    ///  requested services.
    /// </summary>
    public class Container : IServiceProvider, IServiceScope
    {
        private ResolverCache _resolverCache;

        public ILifecycleManager LifecycleManager { get; }

        public IServiceProvider ServiceProvider => this;

        private Container(Container parent)
        {
            Parent = parent;
            Root = parent?.Root ?? this;
            LifecycleManager = new LifecycleManager(this);
        }

        public Container(params IModule[] modules) : this(parent: null)
        {
            _resolverCache = new ResolverCache(modules);
        }


        /* ====== Resolve ====== */

        public object GetService(Type serviceType)
        {
            return GetService(new ServiceRequest(this, serviceType, null));
        }

        private IEnumerable<ServiceResolver> GetResolvers(ServiceRequest serviceRequest)
        {
            return
                from resolver in _resolverCache.GetResolvers(serviceRequest.RequestedType)
                where resolver.PreconditionMet(serviceRequest)
                select resolver;
        }

        private object ResolveFromServicingContainer
            (ServiceResolver resolver, ServiceRequest request)
        {
            var service = resolver.GetService(request);
            if (service == null) return null;
            if (service is IDisposable) {
                var lifecycle = resolver.Registration.Lifecycle;
                var lifecycleManager = lifecycle.GetLifecycleManager(request);
                if (lifecycleManager != null) {
                    lifecycleManager.Add((IDisposable)service);
                }
            }

            return service;
        }

        private object Resolve(ServiceResolver resolver, ServiceRequest request)
        {
            if (!resolver.PreconditionMet(request)) return null;
            var lifecycle = resolver.Registration.Lifecycle;
            return lifecycle.GetServicingContainer(request)
                .ResolveFromServicingContainer(resolver, request);
        }

        public object GetService(ServiceRequest request)
        {
            var resolver = GetResolvers(request).LastOrDefault();
            if (resolver == null) return null;
            return Resolve(resolver, request);
        }

        /* ====== Release ====== */

        public void Dispose()
        {
            LifecycleManager.Dispose();
        }

        /* ====== Hierarchy ====== */

        public Container Parent { get; }

        public Container Root { get; }

        public Container CreateChild()
        {
            var child = new Container(this);
            LifecycleManager.Add(child);
            return child;
        }
    }
}
