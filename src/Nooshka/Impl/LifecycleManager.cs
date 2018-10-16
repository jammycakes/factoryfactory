using System;
using System.Collections.Generic;

namespace Nooshka.Impl
{
    public class LifecycleManager : ILifecycleManager
    {
        private LinkedList<IDisposable> _services = new LinkedList<IDisposable>();
        private IDictionary<Registration, object> _servicesByRegistration
            = new Dictionary<Registration, object>();

        public void Cache(Registration registration, object service)
        {
            _servicesByRegistration.Add(registration, service);
        }

        public void Track(IDisposable service)
        {
            _services.AddFirst(service);
        }

        public object GetExisting(Registration registration)
        {
            if (_servicesByRegistration.TryGetValue(registration, out var obj)) {
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
