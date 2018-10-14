namespace Nooshka.Lifecycle
{
    public class SingletonLifecycle : ILifecycle
    {
        public ILifecycleManager GetLifecycleManager(ServiceRequest request)
        {
            return request.Container.Root.LifecycleManager;
        }

        public Container GetServicingContainer(ServiceRequest request)
        {
            return request.Container.Root;
        }
    }
}
