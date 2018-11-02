namespace FactoryFactory.Resolution
{
    public class InstanceResolver : IResolver
    {
        private readonly object _instance;

        public InstanceResolver(IServiceDefinition definition, object instance)
        {
            _instance = instance;
            Priority = definition.Priority;
        }

        public bool CanResolve => true;

        public bool Conditional => false;

        public int Priority { get; }

        public bool IsConditionMet(ServiceRequest request) => true;

        public object GetService(ServiceRequest request) => _instance;
    }
}
