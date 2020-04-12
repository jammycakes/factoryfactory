using System;
using System.Collections.Generic;

namespace FactoryFactory.Registration.Impl
{
    public interface ITypeFinderBuilder
    {
        Func<Type, IEnumerable<Type>> ToTypeFinder();
    }
}
