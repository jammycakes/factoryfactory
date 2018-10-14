namespace Nooshka.Lifecycles
{
    public class SingletonLifecycle : Lifecycle
    {
        public override ILifecycleManager GetLifecycleManager(ServiceRequest request)
        {
            return request.Container.Root.LifecycleManager;
        }

        public override Container GetServicingContainer(ServiceRequest request)
        {
            return request.Container.Root;
        }
    }
}
