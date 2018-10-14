namespace Nooshka
{
    public interface ILifecycle
    {
        ILifecycleManager GetLifecycleManager(ServiceRequest request);

        Container GetServicingContainer(ServiceRequest request);
    }
}
