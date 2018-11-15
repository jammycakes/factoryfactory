using System;
using System.Reflection;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionPredicates
    {
        IConventionPredicates Where(Predicate<Type> predicate);
    }
}
