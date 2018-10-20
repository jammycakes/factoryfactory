namespace Nooshka.Lifecycles
{
    public class ScopedLifecycle : Lifecycle
    {
        public override IServiceTracker GetTracker(ServiceRequest request)
            => request.Container.ServiceTracker;

        public override IServiceCache GetCache(ServiceRequest request)
            => request.Container.ServiceCache;
    }
}
