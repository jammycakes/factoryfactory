namespace FactoryFactory.Impl
{
    public interface IServiceBuilder
    {
        object GetService(ServiceRequest request);
    }
}
