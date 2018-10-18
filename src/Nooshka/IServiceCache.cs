namespace Nooshka
{
    public interface IServiceCache
    {
        void Store(ServiceDefinition serviceDefinition, object service);

        object Retrieve(ServiceDefinition serviceDefinition);
    }
}
