using System;

namespace FactoryFactory.Registration
{
    public interface IConventionDefinitionClause : IOptionsClause<IConventionDefinitionClause>
    {
        /// <summary>
        ///  Specifies a convention that looks for candidate implementations by
        ///  name.
        /// </summary>
        /// <param name="byName">
        ///  An action that configures the naming conventions.
        /// </param>
        /// <returns></returns>
        Registry As(Action<IConventionByName> byName);

        /// <summary>
        ///  Specifies a convention that scans one or more assemblies for
        ///  all candidate implementations.
        /// </summary>
        /// <param name="byScan">
        ///  An action that configures the assemblies and namespaces to scan
        ///  for candidates.
        /// </param>
        /// <returns></returns>
        Registry Scanning(Action<IConventionByScan> byScan);
    }
}
