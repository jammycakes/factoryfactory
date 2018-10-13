using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Nooshka
{
    public class Module : IModule
    {
        private IDictionary<Type, List<ServiceRegistration>> _registrations;

        public Module(params ServiceRegistration[] registrations)
        {
            var registrationsByType =
                from registration in registrations
                group registration by registration.ServiceType
                into byType
                select byType;

            _registrations = registrationsByType.ToDictionary(k => k.Key, v => v.ToList());
        }

        public Module(IServiceCollection services)
            : this(services.Select(s => new ServiceRegistration(s)).ToArray())
        {
        }

        protected Module()
        {
            _registrations = new Dictionary<Type, List<ServiceRegistration>>();
        }

        private List<ServiceRegistration> GetServiceRegistrations(Type type, bool create)
        {
            List<ServiceRegistration> result = null;
            if (!_registrations.TryGetValue(type, out result)) {
                if (create) {
                    result = new List<ServiceRegistration>();
                    _registrations.Add(type, result);

                }
            }

            return result;
        }

        protected void Add(ServiceRegistration registration)
        {
            var list = GetServiceRegistrations(registration.ServiceType, true);
            list.Add(registration);
        }

        IEnumerable<ServiceRegistration> IModule.GetServiceRegistrations(Type type)
        {
            return (GetServiceRegistrations(type, false) ?? Enumerable.Empty<ServiceRegistration>())
                .AsEnumerable();
        }
    }
}
