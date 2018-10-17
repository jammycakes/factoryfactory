namespace Nooshka.Lifecycles
{
    public class UntrackedLifecycle : Lifecycle
    {
        public override IServiceTracker GetTracker(ServiceRequest request)
            => null;

        public override IServiceCache GetCache(ServiceRequest request)
            => request.ServiceCache;
    }
}
