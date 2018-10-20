using System;

namespace FactoryFactory.Impl
{
    public class DefinitionOptions
    {
        public Lifecycle Lifecycle { get; set; }

        public Func<ServiceRequest, bool> Precondition { get; set; }
    }
}
