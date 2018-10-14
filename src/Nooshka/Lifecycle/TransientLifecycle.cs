namespace Nooshka.Lifecycle
{
    public class TransientLifecycle : ILifecycle
    {
        public ILifecycleManager GetLifecycleManager(ServiceRequest request)
        {
            return null;
        }

        public Container GetServicingContainer(ServiceRequest request)
        {
            return request.Container;
        }
    }
}
