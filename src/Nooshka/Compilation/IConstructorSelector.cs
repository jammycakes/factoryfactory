using System.Reflection;

namespace Nooshka.Compilation
{
    public interface IConstructorSelector
    {
        ConstructorInfo SelectConstructor
            (ServiceDefinition serviceDefinition, Configuration configuration);
    }
}
