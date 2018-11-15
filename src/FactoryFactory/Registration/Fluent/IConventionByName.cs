using System;
using System.Reflection;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionByName : IConventionDefinition<IConventionByName>
    {
        IConventionByName Named(Func<Type, string> naming);
    }
}
