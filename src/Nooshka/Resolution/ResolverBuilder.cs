using System;
using Nooshka.Registration;

namespace Nooshka.Resolution
{
    public class ResolverBuilder
    {
        private readonly IRegistration _registration;

        public ResolverBuilder(IRegistration registration)
        {
            _registration = registration;
        }

        public IServiceResolver Build()
        {
            return null;
        }
    }
}
