using System;
using System.Collections.Generic;
using System.Reflection;
using FactoryFactory.Registration.Fluent;

namespace FactoryFactory.Registration
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
            throw new NotImplementedException();
        }
    }
}
