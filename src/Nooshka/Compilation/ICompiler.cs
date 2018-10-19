using Nooshka.Impl;

namespace Nooshka.Compilation
{
    public interface ICompiler
    {
        IServiceBuilder Build(ServiceDefinition definition, Configuration configuration);
    }
}
