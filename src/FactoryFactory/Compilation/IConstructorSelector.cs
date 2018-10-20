using System.Reflection;

namespace FactoryFactory.Compilation
{
    public interface IConstructorSelector
    {
        ConstructorInfo SelectConstructor
            (ServiceDefinition serviceDefinition, Configuration configuration);
    }
}
