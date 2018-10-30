using System;

namespace FactoryFactory
{
    public class Decorator<TService> : IDecorator<TService> where TService : class
    {
        private readonly Func<ServiceRequest, TService, TService> _proxyFunc;

        public Decorator(Func<ServiceRequest, TService, TService> proxyFunc)
        {
            _proxyFunc = proxyFunc;
        }

        public Decorator(Func<TService, TService> proxyFunc)
        {
            _proxyFunc = (request, service) => proxyFunc(service);
        }

        public TService Decorate(ServiceRequest request, TService service)
        {
            return _proxyFunc(request, service);
        }
    }
}
