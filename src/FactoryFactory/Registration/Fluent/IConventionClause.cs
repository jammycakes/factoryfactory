using System;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionClause
    {
        /// <summary>
        ///  Specifies a convention that looks for candidate implementations by
        ///  name.
        /// </summary>
        /// <param name="byName">
        ///  An action that configures the naming conventions.
        /// </param>
        /// <returns></returns>
        IOptionsClause As(Action<IConventionByName> byName);

        /// <summary>
        ///  Specifies a convention that scans one or more assemblies for
        ///  all candidate implementations.
        /// </summary>
        /// <param name="byScan">
        ///  An action that configures the assemblies and namespaces to scan
        ///  for candidates.
        /// </param>
        /// <returns></returns>
        IOptionsClause Scanning(Action<IConventionByScan> byScan);
    }
}
