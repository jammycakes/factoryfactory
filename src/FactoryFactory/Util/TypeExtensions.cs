using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryFactory.Util
{
    public static class TypeExtensions
    {
        public static Type GetServiceType(this Type type)
        {
            if (type.IsGenericType) {
                var generic = type.GetGenericTypeDefinition();
                if (generic == typeof(IEnumerable<>) || generic == typeof(Func<>)) {
                    return type.GetGenericArguments().Last();
                }
            }

            return type;
        }

        public static bool IsEnumerable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static bool IsFunc(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>);
        }
    }
}
