using System;
using System.Collections.Generic;
using System.Text;

namespace FactoryFactory.Registration.Dsl
{
    public interface IInterceptionDefinitionClause
    {
    }

    public interface IInterceptionDefinitionClause<TService> : IInterceptionDefinitionClause
        where TService : class
    {
    }
}
