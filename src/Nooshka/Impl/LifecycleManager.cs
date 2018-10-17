using System;
using System.Collections.Generic;

namespace Nooshka.Impl
{
    public class LifecycleManager : ILifecycleManager
    {
        private LinkedList<IDisposable> _services = new LinkedList<IDisposable>();
        private IDictionary<ServiceDefinition, object> _servicesByRegistration
            = new Dictionary<ServiceDefinition, object>();

        public void Cache(ServiceDefinition serviceDefinition, object service)
        {
            _servicesByRegistration.Add(serviceDefinition, service);
        }

        public void Track(IDisposable service)
        {
            _services.AddFirst(service);
        }

        public object GetExisting(ServiceDefinition serviceDefinition)
        {
            if (_servicesByRegistration.TryGetValue(serviceDefinition, out var obj)) {
                return obj;
            }

            return null;
        }

        public void Dispose()
        {
            foreach (var service in _services) {
                service.Dispose();
            }
        }
    }
}
