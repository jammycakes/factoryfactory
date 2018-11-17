using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FactoryFactory.Registration.Fluent;
using FactoryFactory.Util;

namespace FactoryFactory.Registration
{
    public class ConventionByScanBuilder : IConventionByScan, ITypeFinderBuilder
    {
        private IList<Func<Type, Assembly>> _assemblyFinders = new List<Func<Type, Assembly>>();
        private IList<Func<Type, string>> _namespaceFinders = new List<Func<Type, string>>();
        private IList<Func<Type, Type, bool>> _filters = new List<Func<Type, Type, bool>>();

        public IConventionByScan FromAssembly(Func<Type, Assembly> assemblyFinder)
        {
            _assemblyFinders.Add(assemblyFinder);
            return this;
        }

        public IConventionByScan FromNamespace(Func<Type, string> namespaceFinder)
        {
            _namespaceFinders.Add(namespaceFinder);
            return this;
        }

        public IConventionByScan Where(Func<Type, Type, bool> filter)
        {
            _filters.Add(filter);
            return this;
        }

        public Func<Type, IEnumerable<Type>> ToTypeFinder()
        {
            return requestedType => {
                var assemblies = _assemblyFinders.Select(af => af(requestedType));
                if (!assemblies.Any()) assemblies = new[] {requestedType.Assembly};
                var namespaces = _namespaceFinders.Select(nf => nf(requestedType));
                if (!namespaces.Any()) namespaces = new[] {"*"};

                return
                    from assembly in assemblies
                    from foundType in AssemblyScan.Instance.GetTypes(assembly)
                    where foundType != null
                          && foundType.IsClass && !foundType.IsAbstract
                          && foundType.InheritsOrImplements(requestedType)
                          && namespaces.Any(nf => foundType.MatchesNamespace(nf))
                          && _filters.All(f => f(requestedType, foundType))
                    select foundType;
            };
        }
    }
}
