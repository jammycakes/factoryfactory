namespace FactoryFactory.Lifecycles
{
    public class SingletonLifecycle : Lifecycle
    {
        public override IServiceTracker GetTracker(ServiceRequest request)
            => request.Container.Root.ServiceTracker;

        public override IServiceCache GetCache(ServiceRequest request)
            => request.Container.Root.ServiceCache;
    }
}
