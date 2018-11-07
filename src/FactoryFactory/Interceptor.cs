using System;

namespace FactoryFactory
{
    public class Interceptor<TService> : IInterceptor<TService> where TService : class
    {
        private readonly Func<ServiceRequest, Func<TService>, TService> _proxyFunc;

        public Interceptor(Func<ServiceRequest, Func<TService>, TService> proxyFunc)
        {
            _proxyFunc = proxyFunc;
        }

        public Interceptor(Func<Func<TService>, TService> proxyFunc)
        {
            _proxyFunc = (request, service) => proxyFunc(service);
        }

        public TService Intercept(ServiceRequest request, Func<TService> service)
        {
            return _proxyFunc(request, service);
        }
    }
}
