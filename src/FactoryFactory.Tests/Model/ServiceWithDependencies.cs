namespace FactoryFactory.Tests.Model
{
    public class ServiceWithDependencies : IServiceWithDependencies
    {
        public string Message { get; }
        public IServiceWithoutDependencies Dependency { get; }

        public ServiceWithDependencies(IServiceWithoutDependencies dependency)
        {
            Dependency = dependency;
        }

        public ServiceWithDependencies(IServiceWithoutDependencies dependency, string message)
            : this(dependency)
        {
            Message = message;
        }
    }
}
