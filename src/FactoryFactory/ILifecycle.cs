namespace FactoryFactory
{
    public interface ILifecycle
    {
        bool Tracked { get; }

        IServiceTracker GetTracker(ServiceRequest request);

        bool Cached { get; }

        IServiceCache GetCache(ServiceRequest request);
    }
}
