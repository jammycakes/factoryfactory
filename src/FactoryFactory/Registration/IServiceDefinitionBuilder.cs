using System;
using System.Collections.Generic;
using System.Text;

namespace FactoryFactory.Registration
{
    public interface IServiceDefinitionBuilder
    {
        IServiceDefinition Build();
    }
}
