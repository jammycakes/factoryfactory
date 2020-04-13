using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FactoryFactory.Registration.Fluent;

namespace FactoryFactory
{
    public static class FluentExtensions
    {
        /* ====== Convention predicates ====== */

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
            => conv.FromAssembly(typeof(T).Assembly);

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
            => conv.FromNamespace(
                typeof(T).Namespace + (includeChildren ? ".*" : ""),
                ignoreCase);

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
            => conv.Where(type => type.GetCustomAttributes<T>().Any());

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


        /* ====== Conventions ====== */

        /// <summary>
        ///  Specifies an assembly in which to look for candidate implementations.
        /// </summary>
        /// <param name="def"></param>
        /// <param name="assembly">
        ///  The assembly containing candidate implementations.
        /// </param>
        /// <typeparam name="TDef"></typeparam>
        /// <returns></returns>
        public static TDef FromAssembly<TDef>(this TDef def, Assembly assembly)
            where TDef : IConventionDefinition<TDef>
            => def.FromAssembly(t => assembly);

        /// <summary>
        ///  Specifies an assembly in which to look for candidate implementations
        ///  by one of the types that it contains.
        /// </summary>
        /// <param name="conv"></param>
        /// <typeparam name="T">
        ///  A type in the assembly containing candidate implementations.
        /// </typeparam>
        /// <returns></returns>
        public static IConventionByName FromAssemblyContaining<T>(this IConventionByName conv)
            => conv.FromAssembly(typeof(T).Assembly);

        /// <summary>
        ///  Specifies an assembly in which to look for candidate implementations
        ///  by one of the types that it contains.
        /// </summary>
        /// <param name="conv"></param>
        /// <typeparam name="T">
        ///  A type in the assembly containing candidate implementations.
        /// </typeparam>
        /// <returns></returns>
        public static IConventionByScan FromAssemblyContaining<T>(this IConventionByScan conv)
            => conv.FromAssembly(typeof(T).Assembly);

        /// <summary>
        ///  Specifies a namespace in which to look for candidate implementations.
        /// </summary>
        /// <param name="def"></param>
        /// <param name="ns">
        ///  The namespace containing candidate implementations.
        /// </param>
        /// <typeparam name="TDef"></typeparam>
        /// <returns></returns>
        public static TDef FromNamespace<TDef>(this TDef def, string ns)
            where TDef : IConventionDefinition<TDef>
            => def.FromNamespace(t => ns);

        /// <summary>
        ///  Specifies a namespace in which to look for candidate implementations
        ///  by one of the types that it contains.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="includeChildren">
        ///  true to include child namespaces, otherwise false.
        /// </param>
        /// <typeparam name="T">
        ///  A type in the namespace containing candidate implementations.
        /// </typeparam>
        /// <returns></returns>
        public static IConventionByName FromNamespaceOf<T>
            (this IConventionByName conv, bool includeChildren = false)
            => conv.FromNamespace(typeof(T).Namespace + (includeChildren ? ".*" : ""));

        /// <summary>
        ///  Specifies a namespace in which to look for candidate implementations
        ///  by one of the types that it contains.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="includeChildren">
        ///  true to include child namespaces, otherwise false.
        /// </param>
        /// <typeparam name="T">
        ///  A type in the assembly containing candidate implementations.
        /// </typeparam>
        /// <returns></returns>
        public static IConventionByScan FromNamespaceOf<T>
            (this IConventionByScan conv, bool includeChildren = false)
            => conv.FromNamespace(typeof(T).Namespace + (includeChildren ? ".*" : ""));

        /// <summary>
        ///  Specifies that candidate implementations must be decorated with
        ///  the given attribute.
        /// </summary>
        /// <param name="conv"></param>
        /// <typeparam name="T">
        ///  The attribute to check for.
        /// </typeparam>
        /// <returns></returns>
        public static IConventionByName WithAttribute<T>(this IConventionByName conv)
            where T : Attribute
            => conv.Where((t1, t2) => t2.GetCustomAttributes<T>().Any());

        /// <summary>
        ///  Specifies that candidate implementations must be decorated with
        ///  the given attribute.
        /// </summary>
        /// <param name="conv"></param>
        /// <typeparam name="T">
        ///  The attribute to check for.
        /// </typeparam>
        /// <returns></returns>
        public static IConventionByScan WithAttribute<T>(this IConventionByScan conv)
            where T : Attribute
            => conv.Where((t1, t2) => t2.GetCustomAttributes<T>().Any());

        /* ====== Convention by name specific ====== */

        /// <summary>
        ///  Specifies that candidate implementations should be found by
        ///  replacing the given search string in the service name by another.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="search">
        ///  The string to search for in the name of the requested service.
        /// </param>
        /// <param name="replace">
        ///  The string to replace it with in the name of the candidate
        ///  implementation.
        /// </param>
        /// <returns></returns>
        public static IConventionByName Replace
            (this IConventionByName conv, string search, string replace)
            => conv.Named(t => t.Name.Replace(search, replace));

        /// <summary>
        ///  Specifies that the candidate implementation should be found by
        ///  replacing the given regular expression in the service name with
        ///  the given replacement pattern.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="search">
        ///  The regular expression to search for in the name of the requested
        ///  service.
        /// </param>
        /// <param name="replace">
        ///  The string or replacement pattern to replace it with in the name
        ///  of the candidate implementation.
        /// </param>
        /// <returns></returns>
        public static IConventionByName Replace
            (this IConventionByName conv, Regex search, string replace)
            => conv.Named(t => search.Replace(t.Name, replace));

        /// <summary>
        ///  Specifies that the candidate implementation should be found by
        ///  replacing the given regular expression in the service name with
        ///  the value computed by the specified <see cref="MatchEvaluator"/>
        ///  instance.
        /// </summary>
        /// <param name="conv"></param>
        /// <param name="search">
        ///  The regular expression to search for in the name of the requested
        ///  service.
        /// </param>
        /// <param name="replace">
        ///  A <see cref="MatchEvaluator"/> function that computes the string
        ///  to replace it with in the name of the candidate implementation.
        /// </param>
        /// <returns></returns>
        public static IConventionByName Replace
            (this IConventionByName conv, Regex search, MatchEvaluator replace)
            => conv.Named(t => search.Replace(t.Name, replace));
    }
}
