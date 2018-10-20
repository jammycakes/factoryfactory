using System;

namespace Nooshka.Lifecycles
{
    public class NullLifecycle : Lifecycle, IServiceTracker, IServiceCache
    {
        public override IServiceTracker GetTracker(ServiceRequest request) => this;

        public override IServiceCache GetCache(ServiceRequest request) => this;

        public void Dispose()
        {
        }

        public void Track(IDisposable service)
        {
        }

        public void Store(ServiceDefinition serviceDefinition, object service)
        {
        }

        public object Retrieve(ServiceDefinition serviceDefinition) => null;
    }
}
