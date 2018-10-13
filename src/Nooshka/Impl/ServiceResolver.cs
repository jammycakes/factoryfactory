using System;

namespace Nooshka.Impl
{
    public class ServiceResolver : IServiceResolver
    {
        private readonly Container _container;

        public ServiceResolver(Container container)
        {
            _container = container;
        }

        public object GetService(ServiceRequest request)
        {
            throw new System.NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
