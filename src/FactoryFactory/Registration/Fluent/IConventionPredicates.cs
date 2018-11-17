using System;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionPredicates
    {
        IConventionPredicates Where(Predicate<Type> predicate);
    }
}
