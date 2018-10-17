using Nooshka.Impl;

namespace Nooshka.Compilation.Expressions
{
    public class RegistrationServiceBuilder : ServiceBuilder
    {
        public RegistrationServiceBuilder(ServiceDefinition definition)
            : base(definition)
        {
        }

        protected override object Resolve(ServiceRequest request)
        {
            return Definition.ImplementationFactory(request);
        }
    }
}
