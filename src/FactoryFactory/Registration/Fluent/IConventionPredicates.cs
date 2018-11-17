using System;

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
    }
}
