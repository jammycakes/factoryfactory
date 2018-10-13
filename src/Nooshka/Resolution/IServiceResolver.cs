namespace Nooshka.Resolution
{
    public interface IServiceResolver
    {
        bool PreconditionMet(ServiceRequest request);

        object GetService(ServiceRequest request);
    }
}
