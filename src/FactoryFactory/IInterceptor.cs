using System;

namespace FactoryFactory
{
    public interface IInterceptor<TService>
    {
        TService Intercept(ServiceRequest request, Func<TService> service);
    }
}
