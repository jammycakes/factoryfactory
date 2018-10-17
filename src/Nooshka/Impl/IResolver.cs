namespace Nooshka.Impl
{
    public interface IResolver
    {
        bool PreconditionMet(ServiceRequest request);
        object GetService(ServiceRequest request);
    }
}