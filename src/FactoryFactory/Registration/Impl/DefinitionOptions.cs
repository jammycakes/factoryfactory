using System;

namespace FactoryFactory.Registration.Impl
{
    public class DefinitionOptions
    {
        public ILifecycle Lifecycle { get; set; }

        public Func<ServiceRequest, bool> Precondition { get; set; }
    }
}
