using Nooshka.Registration;

namespace Nooshka.Resolution
{
    public class RegistrationServiceResolver : IServiceResolver
    {
        private readonly Registration.Registration _registration;

        public RegistrationServiceResolver(Registration.Registration registration)
        {
            _registration = registration;
        }

        public bool PreconditionMet(ServiceRequest request)
        {
            return _registration.Precondition(request);
        }

        public object GetService(ServiceRequest request)
        {
            return _registration.ImplementationFactory(request);
        }
    }
}
