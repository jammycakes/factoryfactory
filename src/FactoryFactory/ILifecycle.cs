namespace FactoryFactory
{
    public interface ILifecycle
    {
        IServiceTracker GetTracker(ServiceRequest request);
        IServiceCache GetCache(ServiceRequest request);
    }
}