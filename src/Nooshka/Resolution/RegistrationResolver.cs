using Nooshka.Registration;

namespace Nooshka.Resolution
{
    public class RegistrationResolver : Resolver
    {
        public RegistrationResolver(ServiceRegistration registration)
            : base(registration)
        {
        }

        public override object GetService(ServiceRequest request)
        {
            return Registration.ImplementationFactory(request);
        }
    }
}
