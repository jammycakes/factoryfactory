using Nooshka.Impl;

namespace Nooshka.Compilation.Expressions
{
    public class RegistrationResolver : ResolverBase
    {
        public RegistrationResolver(ServiceDefinition definition)
            : base(definition)
        {
        }

        protected override object Resolve(ServiceRequest request)
        {
            return Definition.ImplementationFactory(request);
        }
    }
}
