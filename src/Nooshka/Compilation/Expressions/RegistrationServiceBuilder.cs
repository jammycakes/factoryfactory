using System;
using System.Runtime.InteropServices;
using Nooshka.Impl;

namespace Nooshka.Compilation.Expressions
{
    public class RegistrationServiceBuilder : ServiceBuilder
    {
        private Func<ServiceRequest, object> _factory;

        public RegistrationServiceBuilder(ServiceDefinition definition)
            : base(definition)
        {
            _factory = definition.ImplementationFactory.Compile();
        }

        protected override object Resolve(ServiceRequest request)
        {
            return _factory(request);
        }
    }
}
