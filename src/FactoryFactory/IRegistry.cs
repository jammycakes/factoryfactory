using System.Collections.Generic;

namespace FactoryFactory
{
    public interface IRegistry
    {
        IEnumerable<IServiceDefinition> GetServiceDefinitions();
    }
}
