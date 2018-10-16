using Nooshka.Registration;

namespace Nooshka.Resolution
{
    public class RegistrationServiceResolver : ServiceResolver
    {
        public RegistrationServiceResolver(ServiceRegistration registration)
            : base(registration)
        {
        }

        public override object GetService(ServiceRequest request)
        {
            return Registration.ImplementationFactory(request);
        }
    }
}
