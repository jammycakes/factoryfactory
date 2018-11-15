using System;
using System.Collections.Generic;
using System.Reflection;
using FactoryFactory.Registration.Fluent;

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
            throw new NotImplementedException();
        }
    }
}
