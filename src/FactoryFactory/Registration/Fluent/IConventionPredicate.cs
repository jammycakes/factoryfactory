using System;
using System.Reflection;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionPredicate
    {
        IConventionPredicate Where(Predicate<Type> predicate);
    }
}
