using FactoryFactory.Impl;

namespace FactoryFactory.Compilation
{
    public interface ICompiler
    {
        IServiceBuilder Build(ServiceDefinition definition, Configuration configuration);
    }
}
