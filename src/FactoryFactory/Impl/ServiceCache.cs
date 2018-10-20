using System.Collections.Generic;

namespace FactoryFactory.Impl
{
    public class ServiceCache : IServiceCache
    {
        private IDictionary<ServiceDefinition, object> _servicesByDefinition
            = new Dictionary<ServiceDefinition, object>();

        public void Store(ServiceDefinition serviceDefinition, object service)
        {
            _servicesByDefinition.Add(serviceDefinition, service);
        }

        public object Retrieve(ServiceDefinition serviceDefinition)
        {
            if (_servicesByDefinition.TryGetValue(serviceDefinition, out var obj)) {
                return obj;
            }

            return null;
        }
    }
}
