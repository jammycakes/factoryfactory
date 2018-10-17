using Nooshka.Impl;

namespace Nooshka.Compilation
{
    public interface IResolverCompiler
    {
        IResolver Build(ServiceDefinition definition, Configuration configuration);
    }
}
