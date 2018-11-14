using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FactoryFactory.Util
{
    public class AssemblyScan
    {
        private IDictionary<Assembly, ReadOnlyCollection<Type>> _typesByAssembly
            = new Dictionary<Assembly, ReadOnlyCollection<Type>>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ICollection<Type> GetTypes(Assembly assembly)
        {
            if (!_typesByAssembly.TryGetValue(assembly, out var types)) {
                try {
                    types = new ReadOnlyCollection<Type>(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException rex) {
                    types = new ReadOnlyCollection<Type>(rex.Types.Where(t => t != null).ToList());
                }
                _typesByAssembly.Add(assembly, types);
            }

            return types;
        }

        public static AssemblyScan Instance { get; }

        static AssemblyScan()
        {
            Instance = new AssemblyScan();
        }
    }
}
