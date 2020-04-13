using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using FactoryFactory.Util;

namespace FactoryFactory.Registration.Impl
{
    public class ConventionPredicates : IConventionPredicates
    {
        private IList<Predicate<Type>> _predicates = new List<Predicate<Type>>();
        private IList<Assembly> _assemblies = new List<Assembly>();
        private IList<(string, bool)> _namespaces = new List<(string, bool)>();

        public IConventionPredicates Where(Predicate<Type> predicate)
        {
            _predicates.Add(predicate);
            return this;
        }

        public IConventionPredicates FromAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
            return this;
        }

        public IConventionPredicates FromNamespace(string namespacePattern, bool ignoreCase = false)
        {
            _namespaces.Add((namespacePattern, ignoreCase));
            return this;
        }

        public Predicate<Type> ToPredicate()
        {
            var predicates = new ReadOnlyCollection<Predicate<Type>>(_predicates);
            var assemblies = new ReadOnlyCollection<Assembly>(_assemblies);
            var namespaces = new ReadOnlyCollection<(string, bool)>(_namespaces);

            return requestedType => {
                if (assemblies.Any() && assemblies.All(a => a != requestedType.Assembly))
                    return false;
                if (namespaces.Any() &&
                    namespaces.All(n => !requestedType.MatchesNamespace(n.Item1, n.Item2)))
                    return false;
                return predicates.All(p => p(requestedType));
            };
        }
    }
}
