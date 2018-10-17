using Nooshka.Impl;

namespace Nooshka.Compilation
{
    public interface IResolverCompiler
    {
        Resolver Build(ServiceDefinition definition, Configuration configuration);
    }
}
