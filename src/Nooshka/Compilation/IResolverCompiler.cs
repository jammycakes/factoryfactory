using Nooshka.Impl;

namespace Nooshka.Compilation
{
    public interface IResolverCompiler
    {
        IServiceBuilder Build(ServiceDefinition definition, Configuration configuration);
    }
}
