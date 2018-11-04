using System;
using System.Reflection;

namespace FactoryFactory.Expressions
{
    public interface IConstructorSelector
    {
        ConstructorInfo SelectConstructor(Type implementationType, Configuration configuration);
    }
}
