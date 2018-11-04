using System;

namespace FactoryFactory.Resolution
{
    public interface IResolverBuilder
    {
        Type EnumerableType { get; }

        Type InstanceType { get; }

        IResolver GetEnumerableResolver();

        IResolver GetInstanceResolver();
    }
}
