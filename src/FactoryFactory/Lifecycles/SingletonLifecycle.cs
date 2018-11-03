namespace FactoryFactory.Lifecycles
{
    public class SingletonLifecycle : ILifecycle
    {
        public bool Cached => true;

        public bool Tracked => true;

        public IServiceCache GetCache(ServiceRequest request)
            => request.Container.Root.ServiceCache;

        public IServiceTracker GetTracker(ServiceRequest request)
            => request.Container.Root.ServiceTracker;
    }
}
