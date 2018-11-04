using System;
using System.Reflection;

namespace FactoryFactory.Compilation
{
    public interface IConstructorSelector
    {
        ConstructorInfo SelectConstructor(Type implementationType, Configuration configuration);
    }
}
