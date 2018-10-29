using System;

namespace FactoryFactory.Registration
{
    public class DefinitionOptions
    {
        public Lifecycle Lifecycle { get; set; }

        public Func<ServiceRequest, bool> Precondition { get; set; }

        public Func<ServiceRequest, object, object> Decorator { get; set; }
    }
}
