using System.Collections.Generic;

namespace FactoryFactory.Tests.Model
{
    public class ServiceWithEnumerableDependencies
    {
        public IEnumerable<IServiceWithoutDependencies> Dependencies { get; }

        public ServiceWithEnumerableDependencies(IEnumerable<IServiceWithoutDependencies> dependencies)
        {
            Dependencies = dependencies;
        }
    }
}
