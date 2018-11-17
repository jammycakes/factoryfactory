using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FactoryFactory.Registration.Fluent;
using FactoryFactory.Util;

namespace FactoryFactory
{
    public static class FluentExtensions
    {
        /* ====== IOptionsClause ====== */

        /// <summary>
        ///  Configures the service as a singleton.
        /// </summary>
        /// <returns></returns>
        public static IOptionsClause Singleton(this IOptionsClause options)
            => options.Lifecycle(FactoryFactory.Lifecycle.Singleton);

        /// <summary>
        ///  Configures the service as a transient.
        /// </summary>
        /// <returns></returns>
        public static IOptionsClause Transient(this IOptionsClause options)
            => options.Lifecycle(FactoryFactory.Lifecycle.Transient);

        /// <summary>
        ///  Configures the service as a scoped service.
        /// </summary>
        /// <returns></returns>
        public static IOptionsClause Scoped(this IOptionsClause options)
            => options.Lifecycle(FactoryFactory.Lifecycle.Scoped);

        /// <summary>
        ///  Configures the service as an untracked service.
        ///  IDisposable.Dispose() will not be called when the container is
        ///  disposed.
        /// </summary>
        /// <returns></returns>
        public static IOptionsClause Untracked(this IOptionsClause options)
            => options.Lifecycle(FactoryFactory.Lifecycle.Untracked);


        /* ====== Convention predicates ====== */

        /// <summary>
        ///  Configures a convention to apply to requests for services defined
        ///  in the given assembly.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="assembly">
        ///  The assembly containing the services to be defined by this
        ///  convention.
        /// </param>
        /// <returns></returns>
        public static IConventionPredicates FromAssembly
            (this IConventionPredicates conv, Assembly assembly)
            => conv.Where(type => type.Assembly == assembly);

        /// <summary>
        ///  Configures a convention to apply to requests for services defined
        ///  in the given assemblies.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="assemblies">
        ///  The assemblies containing the services to be defined by this
        ///  convention.
        /// </param>
        /// <returns></returns>
        public static IConventionPredicates FromAssemblies
            (this IConventionPredicates conv, params Assembly[] assemblies)
            => conv.Where(type => assemblies.Contains(type.Assembly));

        /// <summary>
        ///  Configures a convention to apply to requests for services defined
        ///  in the assembly containing the given type.
        /// </summary>
        /// <param name="conv"></param>
        /// <typeparam name="T">
        ///  The type whose assembly contains the services to be defined by this
        ///  convention.
        /// </typeparam>
        /// <returns></returns>
        public static IConventionPredicates FromAssemblyContaining<T>
            (this IConventionPredicates conv)
            => conv.Where(type => type.Assembly == typeof(T).Assembly);

        /// <summary>
        ///  Configures a convention to apply to requests for services defined
        ///  in the given namespace.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="namespacePattern">
        ///  The namespace to match. Wildcards may be used.
        /// </param>
        /// <returns></returns>
        public static IConventionPredicates FromNamespace
            (this IConventionPredicates conv, string namespacePattern, bool ignoreCase = false)
            => conv.Where(type => type.MatchesNamespace(namespacePattern, ignoreCase));

        /// <summary>
        ///  Configures a convention to apply to requests for services defined
        ///  in the namespace of the given type, and optionally also in
        ///  child namespaces.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="includeChildren">
        ///  Whether or not to include child namespaces.
        /// </param>
        /// <typeparam name="T">
        ///  The type whose namespace contains the services defined for this
        ///  convention.
        /// </typeparam>
        /// <returns></returns>
        public static IConventionPredicates FromNamespaceOf<T>
            (this IConventionPredicates conv, bool includeChildren = false, bool ignoreCase = false)
            => conv.Where(type =>
                type.MatchesNamespace(
                    typeof(T).Namespace + (includeChildren ? ".*" : ""),
                    ignoreCase));

        /// <summary>
        ///  Configures a convention to apply only to requests decorated with
        ///  the specified attribute.
        /// </summary>
        /// <param name="conv"></param>
        /// <typeparam name="T">
        ///  The attribute to check for.
        /// </typeparam>
        /// <returns></returns>
        public static IConventionPredicates WithAttribute<T>(this IConventionPredicates conv)
            where T : Attribute
            => conv.Where(type => type.GetCustomAttribute<T>() != null);

        /// <summary>
        ///  Configures a convention to apply only to requests for interfaces.
        /// </summary>
        /// <param name="conv"></param>
        /// <returns></returns>
        public static IConventionPredicates Interfaces(this IConventionPredicates conv)
            => conv.Where(type => type.IsInterface);

        /// <summary>
        ///  Configures a convention to apply only to requests for services
        ///  whose name starts with the given prefix.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="prefix">
        ///  The name prefix to check for.
        /// </param>
        /// <param name="ignoreCase">
        ///  true to ignore case, otherwise false.
        /// </param>
        /// <returns></returns>
        public static IConventionPredicates WithNameStarting
            (this IConventionPredicates conv, string prefix, bool ignoreCase = false)
            => conv.Where(type =>
                type.Name.StartsWith(
                    prefix,
                    ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));

        /// <summary>
        ///  Configures a convention to apply only to requests for services
        ///  whose names match the given regular expression.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="re">
        ///  The regular expression to match.
        /// </param>
        /// <returns></returns>
        public static IConventionPredicates WithNameMatching
            (this IConventionPredicates conv, Regex re)
            => conv.Where(type => re.IsMatch(type.Name));

        /// <summary>
        ///  Configures a convention to apply only to requests for services
        ///  whose names match the given regular expression.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="re">
        ///  The regular expression string to match.
        /// </param>
        /// <param name="options">
        ///  The <see cref="RegexOptions"/> to apply to the regular expression.
        /// </param>
        /// <returns></returns>
        public static IConventionPredicates WithNameMatching
            (this IConventionPredicates conv, string re, RegexOptions options = RegexOptions.None)
            => conv.WithNameMatching(new Regex(re, options));


        /// <summary>
        ///  Configures a convention to apply only to requests for services
        ///  whose full names (including namespace) match the given regular
        ///  expression.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="re">
        ///  The regulqr expression to match.
        /// </param>
        /// <returns></returns>
        public static IConventionPredicates WithFullNameMatching
            (this IConventionPredicates conv, Regex re)
            => conv.Where(type => re.IsMatch(type.FullName));

        /// <summary>
        ///  Configures a convention to apply only to requests for services
        ///  whose full names (including namespace) match the given regular
        ///  expression.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="re">
        ///  The regular expression string to match.
        /// </param>
        /// <param name="options">
        ///  The <see cref="RegexOptions"/> to apply to the regular expression.
        /// </param>
        /// <returns></returns>
        public static IConventionPredicates WithFullNameMatching
            (this IConventionPredicates conv, string re, RegexOptions options = RegexOptions.None)
            => conv.WithFullNameMatching(new Regex(re, options));
    }
}
