using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Impl;

namespace Nooshka
{
    /// <summary>
    ///  The core class which is responsible for resolution and management of
    ///  requested services.
    /// </summary>
    public class Container : IServiceProvider, IServiceScope, IServiceScopeFactory
    {
        private readonly Configuration _configuration;

        public ILifecycleManager LifecycleManager { get; }

        public IServiceProvider ServiceProvider => this;

        private Container(Container parent)
        {
            Parent = parent;
            Root = parent?.Root ?? this;
            LifecycleManager = new LifecycleManager();
            _configuration = Root._configuration;
        }

        internal Container(Configuration configuration) : this(parent: null)
        {
            _configuration = configuration;
        }


        /* ====== Resolve ====== */

        public object GetService(Type serviceType)
        {
            var request = new ServiceRequest(this, serviceType, null);
            return GetService(request);
        }

        public object GetService(ServiceRequest request)
        {
            if (request.GenericType == typeof(IEnumerable<>)) {
                return GetAll(request);
            }
            else {
                return GetOne(request);
            }
        }

        private object GetAll(ServiceRequest request)
        {
            var resolvers = GetResolvers(request);
            var services = resolvers.Select(r => r.GetService(request)).ToArray();
            var result = Array.CreateInstance(request.ServiceType, services.Length);
            Array.Copy(services, result, services.Length);
            return result;
        }

        private object GetOne(ServiceRequest request)
        {
            var resolver = GetResolvers(request).LastOrDefault();
            if (resolver == null) return null;
            return resolver.GetService(request);
        }


        private IEnumerable<Resolver> GetResolvers(ServiceRequest serviceRequest)
        {
            return
                from resolver in _configuration.GetResolvers(serviceRequest.ServiceType)
                where resolver.PreconditionMet(serviceRequest)
                select resolver;
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
            LifecycleManager.Track(child);
            return child;
        }

        IServiceScope IServiceScopeFactory.CreateScope()
        {
            return CreateChild();
        }
    }
}
