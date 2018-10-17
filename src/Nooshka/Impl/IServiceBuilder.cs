namespace Nooshka.Impl
{
    public interface IServiceBuilder
    {
        bool PreconditionMet(ServiceRequest request);
        object GetService(ServiceRequest request);
    }
}
