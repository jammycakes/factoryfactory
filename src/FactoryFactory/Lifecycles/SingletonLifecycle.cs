namespace FactoryFactory.Lifecycles
{
    public class SingletonLifecycle : Lifecycle
    {
        public override bool Cached => true;

        public override bool Tracked => true;

        public override IServiceCache GetCache(ServiceRequest request)
            => request.Container.Root.ServiceCache;

        public override IServiceTracker GetTracker(ServiceRequest request)
            => request.Container.Root.ServiceTracker;
    }
}
