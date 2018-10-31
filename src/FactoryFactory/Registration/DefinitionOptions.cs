using System;

namespace FactoryFactory.Registration
{
    public class DefinitionOptions
    {
        public ILifecycle Lifecycle { get; set; }

        public Func<ServiceRequest, bool> Precondition { get; set; }
    }
}
