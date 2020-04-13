using System;

namespace FactoryFactory.Registration
{
    public interface IConventionByName : IConventionDefinition<IConventionByName>
    {
        /// <summary>
        ///  Specifies the name for an implementation of the given service.
        /// </summary>
        /// <param name="naming">
        ///  A function that, given a requested service type, constructs a name
        ///  for its possible implementation, e.g. IRepository => Repository
        /// </param>
        /// <returns></returns>
        IConventionByName Named(Func<Type, string> naming);
    }
}
