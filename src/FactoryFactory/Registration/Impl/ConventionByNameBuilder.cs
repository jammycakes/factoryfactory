using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FactoryFactory.Util;

namespace FactoryFactory.Registration.Impl
{
    public class ConventionByNameBuilder : IConventionByName, ITypeFinderBuilder
    {
        private IList<Func<Type, Assembly>> _assemblyFinders = new List<Func<Type, Assembly>>();
        private IList<Func<Type, string>> _namespaceFinders = new List<Func<Type, string>>();
        private IList<Func<Type, Type, bool>> _filters = new List<Func<Type, Type, bool>>();
        private IList<Func<Type, string>> _namings = new List<Func<Type, string>>();

        public IConventionByName FromAssembly(Func<Type, Assembly> assemblyFinder)
        {
            _assemblyFinders.Add(assemblyFinder);
            return this;
        }

        public IConventionByName FromNamespace(Func<Type, string> namespaceFinder)
        {
            _namespaceFinders.Add(namespaceFinder);
            return this;
        }

        public IConventionByName Where(Func<Type, Type, bool> filter)
        {
            _filters.Add(filter);
            return this;
        }

        public IConventionByName Named(Func<Type, string> naming)
        {
            _namings.Add(naming);
            return this;
        }

        Func<Type, IEnumerable<Type>> ITypeFinderBuilder.ToTypeFinder()
        {
            return requestedType => {
                var assemblies = _assemblyFinders.Select(af => af(requestedType));
                if (!assemblies.Any()) assemblies = new[] {requestedType.Assembly};
                var namespaces = _namespaceFinders.Select(nf => nf(requestedType));
                if (!namespaces.Any()) namespaces = new[] {requestedType.Namespace};
                var names = _namings.Select(n => n(requestedType));

                return
                    from assembly in assemblies
                    from ns in namespaces
                    from name in names
                    let foundType = assembly.GetType(ns + "." + name, false, true)
                    where foundType != null
                          && foundType.IsClass && !foundType.IsAbstract
                          && foundType.InheritsOrImplements(requestedType)
                          && _filters.All(f => f(requestedType, foundType))
                    select foundType;
            };
        }
    }
}
