using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FactoryFactory.Impl;
using FactoryFactory.Util;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{

    /// <summary>
    ///  The core class which is responsible for resolution and management of
    ///  requested services.
    /// </summary>
    internal class Container : IContainer
    {
        private readonly Configuration _configuration;

        public IServiceCache ServiceCache { get; }

        public IServiceTracker ServiceTracker { get; }

        public IServiceProvider ServiceProvider => this;

        private Container(IContainer parent, Configuration configuration)
        {
            Parent = parent;
            Root = parent?.Root ?? this;
            ServiceCache = new ServiceCache();
            ServiceTracker = new ServiceTracker();
            _configuration = configuration;
        }

        public Container(Configuration configuration) : this(null, configuration)
        {
        }


        /* ====== Resolve ====== */

        public object GetService(Type serviceType)
        {
            var request = new ServiceRequest(this, serviceType, null);
            return GetService(request);
        }

        public object GetService(ServiceRequest request)
        {
            if (request.IsEnumerable) {
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
            var result = Array.CreateInstance(request.RequestedType.GetServiceType(), services.Length);
            Array.Copy(services, result, services.Length);
            return result;
        }

        private object GetOne(ServiceRequest request)
        {
            var resolver = GetResolver(request);
            if (resolver == null) return null;
            return resolver.GetService(request);
        }

        private IServiceResolver GetResolver(ServiceRequest serviceRequest)
        {
            IServiceResolver open = null;
            IServiceResolver closed = null;

            foreach (var resolver in GetResolvers(serviceRequest)) {
                if (resolver.IsOpenGeneric) {
                    open = resolver;
                }
                else {
                    closed = resolver;
                }
            }

            return closed ?? open;
        }


        private IEnumerable<IServiceResolver> GetResolvers(ServiceRequest serviceRequest)
        {
            return
                from resolver in _configuration.GetResolvers(serviceRequest.RequestedType.GetServiceType())
                where resolver.PreconditionMet(serviceRequest)
                select resolver;
        }


        /* ====== Release ====== */

        private bool _disposing = false;

        public void Dispose()
        {
            if (_disposing) return;
            _disposing = true;
            try {
                ServiceTracker.Dispose();
            }
            finally {
                _disposing = false;
            }
        }

        /* ====== Hierarchy ====== */

        public IContainer Parent { get; }

        public IContainer Root { get; }

        public IContainer CreateChild()
        {
            var child = new Container(this, _configuration);
            return child;
        }

        IServiceScope IServiceScopeFactory.CreateScope()
        {
            return CreateChild();
        }
    }
}
