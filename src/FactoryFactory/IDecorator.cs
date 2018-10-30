namespace FactoryFactory
{
    public interface IDecorator<TService>
    {
        TService Decorate(ServiceRequest request, TService service);
    }
}
