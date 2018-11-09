using System;

namespace FactoryFactory.Tests.Model
{
    public class InterceptorWithTypeConstraints<TService> : IInterceptor<TService>
        where TService: IBlueService
    {
        public TService Intercept(ServiceRequest request, Func<TService> service)
        {
            var result = service();
            result.Intercepted = true;
            return result;
        }
    }
}
