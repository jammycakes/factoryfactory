namespace Nooshka
{
    public interface ILifetime
    {
        ILifetimeManager GetLifetimeManager(ServiceRequest request);
    }
}
