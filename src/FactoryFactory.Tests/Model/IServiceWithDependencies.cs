namespace FactoryFactory.Tests.Model
{
    public interface IServiceWithDependencies
    {
        IServiceWithoutDependencies Dependency { get; }
        string Message { get; }
    }
}
