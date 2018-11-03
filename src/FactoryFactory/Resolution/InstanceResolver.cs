using System;

namespace FactoryFactory.Resolution
{
    public class InstanceResolver : IResolver
    {
        private readonly object _instance;

        public InstanceResolver(IServiceDefinition definition, object instance)
        {
            _instance = instance;
            Priority = definition.Priority;
            Type = instance.GetType();
        }

        public bool CanResolve => true;

        public bool Conditional => false;

        public int Priority { get; }

        public Type Type { get; }

        public bool IsConditionMet(ServiceRequest request) => true;

        public object GetService(ServiceRequest request) => _instance;

        public override string ToString() => $"InstanceResolver for {Type}";
    }
}
