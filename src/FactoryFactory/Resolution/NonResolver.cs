using System;

namespace FactoryFactory.Resolution
{
    /// <summary>
    ///  A placeholder instance of <see cref="IResolver"/>, cached to indicate
    ///  to the container that the requested service can not be resolved.
    /// </summary>

    public class NonResolver : IResolver
    {
        public NonResolver(Type type)
        {
            Type = type;
        }

        public bool CanResolve => false;

        public bool Conditional => false;

        public object Key => null;

        public int Priority => 0;

        public Type Type { get; }

        public bool IsConditionMet(ServiceRequest request) => false;

        public object GetService(ServiceRequest request) => null;

        public override string ToString() => $"Non-resolver for {Type}";
    }
}
