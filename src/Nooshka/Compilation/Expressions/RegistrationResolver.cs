using Nooshka.Impl;

namespace Nooshka.Compilation.Expressions
{
    public class RegistrationResolver : ResolverBase
    {
        public RegistrationResolver(ServiceDefinition serviceDefinition)
            : base(serviceDefinition)
        {
        }

        protected override object Resolve(ServiceRequest request)
        {
            return ServiceDefinition.ImplementationFactory(request);
        }
    }
}
