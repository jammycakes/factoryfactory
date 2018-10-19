namespace Nooshka.Impl
{
    public interface IServiceResolver
    {
        bool PreconditionMet(ServiceRequest request);
        object GetService(ServiceRequest request);
    }
}
