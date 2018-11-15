using System;
using System.Collections.Generic;
using System.Reflection;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionDefinition<TReturn>
        where TReturn: IConventionDefinition<TReturn>
    {
        TReturn FromAssembly(Func<Type, Assembly> assemblyFinder);
        TReturn FromNamespace(Func<Type, string> namespaceFinder);
        TReturn Where(Func<Type, Type, bool> filter);
    }
}
