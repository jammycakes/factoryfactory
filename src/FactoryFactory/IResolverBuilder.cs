using System;

namespace FactoryFactory
{
    public interface IResolverBuilder
    {
        Type EnumerableType { get; }

        Type InstanceType { get; }

        IResolver GetEnumerableResolver();

        IResolver GetInstanceResolver();
    }
}
