namespace FactoryFactory.Impl
{
    public interface IServiceResolver
    {
        bool IsOpenGeneric { get; }

        bool PreconditionMet(ServiceRequest request);
        object GetService(ServiceRequest request);
    }
}
