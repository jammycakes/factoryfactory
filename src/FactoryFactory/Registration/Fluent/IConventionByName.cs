using System;
using System.Reflection;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionByName
    {
        IConventionByName FromAssembly(Func<Type, Assembly> assemblyFinder);
        IConventionByName FromNamespace(Func<Type, string> namespaceFinder);
        IConventionByName Where(Func<Type, Type, bool> filter);
        IConventionByName Named(Func<Type, string> naming);
    }
}
