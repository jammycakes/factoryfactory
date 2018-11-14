using System;
using System.Reflection;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionByScan
    {
        IConventionByScan FromAssembly(Func<Type, Assembly> assemblyFinder);
        IConventionByScan FromNamespace(Func<Type, string> namespaceFinder);
        IConventionByScan Where(Func<Type, Type, bool> filter);
    }
}
