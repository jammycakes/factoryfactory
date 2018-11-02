namespace FactoryFactory
{
    public interface IServiceCache
    {
        void Store(object key, object service);

        object Retrieve(object key);
    }
}
