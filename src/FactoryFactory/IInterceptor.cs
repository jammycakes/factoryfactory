using System;

namespace FactoryFactory
{
    public interface IInterceptor<TService>
    {
        Func<ServiceRequest, TService> Intercept(Func<ServiceRequest, TService> func);
    }
}
