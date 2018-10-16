namespace Nooshka.Impl
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
