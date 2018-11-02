using System.Collections.Generic;

namespace FactoryFactory.Impl
{
    public class ServiceCache : IServiceCache
    {
        private IDictionary<object, object> _servicesByDefinition
            = new Dictionary<object, object>();

        public void Store(object key, object service)
        {
            _servicesByDefinition.Add(key, service);
        }

        public object Retrieve(object key)
        {
            if (_servicesByDefinition.TryGetValue(key, out var obj)) {
                return obj;
            }

            return null;
        }
    }
}
