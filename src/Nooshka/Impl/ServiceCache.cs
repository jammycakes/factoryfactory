using System;
using System.Collections.Generic;

namespace Nooshka.Impl
{
    public class ServiceCache : IServiceCache
    {
        private IDictionary<ServiceDefinition, object> _servicesByRegistration
            = new Dictionary<ServiceDefinition, object>();

        public void Store(ServiceDefinition serviceDefinition, object service)
        {
            _servicesByRegistration.Add(serviceDefinition, service);
        }

        public object Retrieve(ServiceDefinition serviceDefinition)
        {
            if (_servicesByRegistration.TryGetValue(serviceDefinition, out var obj)) {
                return obj;
            }

            return null;
        }
    }
}
