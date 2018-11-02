namespace FactoryFactory.Lifecycles
{
    public class ScopedLifecycle : Lifecycle
    {
        public override bool Cached => true;

        public override bool Tracked => true;

        public override IServiceTracker GetTracker(ServiceRequest request)
            => request.Container.ServiceTracker;

        public override IServiceCache GetCache(ServiceRequest request)
            => request.Container.ServiceCache;
    }
}
