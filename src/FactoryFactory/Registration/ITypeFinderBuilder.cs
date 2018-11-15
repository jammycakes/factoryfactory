using System;
using System.Collections.Generic;

namespace FactoryFactory.Registration
{
    public interface ITypeFinderBuilder
    {
        Func<Type, IEnumerable<Type>> ToTypeFinder();
    }
}
