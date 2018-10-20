using System;

namespace FactoryFactory.Tests.Model
{
    public class ServiceWithLazyDependencies : IServiceWithDependencies
    {
        private readonly Lazy<IServiceWithoutDependencies> _dependency;
        public IServiceWithoutDependencies Dependency => _dependency.Value;
        public string Message { get; }

        public ServiceWithLazyDependencies(Lazy<IServiceWithoutDependencies> dependency)
        {
            _dependency = dependency;
        }
    }
}
