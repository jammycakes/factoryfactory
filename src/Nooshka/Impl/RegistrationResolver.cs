namespace Nooshka.Impl
{
    public class RegistrationResolver : Resolver
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
