using System;
using System.Reflection;

namespace FactoryFactory.Registration
{
    public interface IConventionDefinition<TReturn>
        where TReturn: IConventionDefinition<TReturn>
    {
        /// <summary>
        ///  Specifies an assembly to look for candidate implementations.
        /// </summary>
        /// <param name="assemblyFinder">
        ///  A function that, given a requested service type, returns an
        ///  assembly to look for candidates.
        /// </param>
        /// <returns></returns>
        TReturn FromAssembly(Func<Type, Assembly> assemblyFinder);

        /// <summary>
        ///  Specifies a namespace to look for candidate implementations.
        /// </summary>
        /// <param name="namespaceFinder">
        ///  A function that, given a requested service type, returns a
        ///  namespace to look for candidates. For name-based conventions, this
        ///  must be an exact match. For scan-based conventions, wildcards can
        ///  be used.
        /// </param>
        /// <returns></returns>
        TReturn FromNamespace(Func<Type, string> namespaceFinder);

        /// <summary>
        ///  Specifies an additional criterion that candidate implementations
        ///  must meet (for example, having a specific attribute).
        /// </summary>
        /// <param name="filter">
        ///  A function that, given a requested service type in the first
        ///  argument and a candidate implementation in the second, confirms
        ///  or rejects it based on the specified criterion.
        /// </param>
        /// <returns></returns>
        TReturn Where(Func<Type, Type, bool> filter);
    }
}
