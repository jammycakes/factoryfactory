using System;

namespace FactoryFactory.Lifecycles
{
    public class UntrackedLifecycle : Lifecycle, IServiceTracker
    {
        public override IServiceTracker GetTracker(ServiceRequest request)
            => this;

        public override IServiceCache GetCache(ServiceRequest request)
            => request.ServiceCache;

        public void Dispose()
        {
        }

        public void Track(IDisposable service)
        {
        }
    }
}
