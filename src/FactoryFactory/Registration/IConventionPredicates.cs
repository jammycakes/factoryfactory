using System;
using System.Reflection;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionPredicates
    {
        /// <summary>
        ///  Specifies a criterion that requested service types must meet in
        ///  order to be included in this convention.
        /// </summary>
        /// <param name="predicate">
        ///  A function that confirms or rejects a type based on the specified
        ///  criterion.
        /// </param>
        /// <returns></returns>
        IConventionPredicates Where(Predicate<Type> predicate);

        /// <summary>
        ///  Configures a convention to apply to requests for services defined
        ///  in the given assembly.
        /// </summary>
        /// <param name="assembly">
        ///  The assembly containing the services to be defined by this
        ///  convention.
        /// </param>
        /// <returns></returns>
        IConventionPredicates FromAssembly(Assembly assembly);

        /// <summary>
        ///  Configures a convention to apply to requests for services defined
        ///  in the given namespace.
        /// </summary>
        /// <param name="namespacePattern">
        ///  The namespace to match. Wildcards may be used.
        /// </param>
        /// <param name="ignoreCase">
        ///  Indicates whether or not case should be ignored when matching
        ///  namespaces.
        /// </param>
        /// <returns></returns>
        IConventionPredicates FromNamespace(string namespacePattern, bool ignoreCase = false);
    }
}
