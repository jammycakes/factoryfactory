using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FactoryFactory.Util
{
    public static class TypeExtensions
    {
        public static Type GetServiceType(this Type type)
        {
            if (type.IsGenericType) {
                var generic = type.GetGenericTypeDefinition();
                if (generic == typeof(IEnumerable<>)) {
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

        public static bool InheritsOrImplements(this Type child, Type parent)
        {
            parent = ResolveGenericTypeDefinition(parent);

            var currentChild = child.IsGenericType
                ? child.GetGenericTypeDefinition()
                : child;

            while (currentChild != typeof(object)) {
                if (parent == currentChild || HasAnyInterfaces(parent, currentChild))
                    return true;

                currentChild = currentChild.BaseType != null
                               && currentChild.BaseType.IsGenericType
                    ? currentChild.BaseType.GetGenericTypeDefinition()
                    : currentChild.BaseType;

                if (currentChild == null)
                    return false;
            }

            return false;
        }

        private static bool HasAnyInterfaces(Type parent, Type child)
        {
            return child.GetInterfaces()
                .Any(childInterface => {
                    var currentInterface = childInterface.IsGenericType
                        ? childInterface.GetGenericTypeDefinition()
                        : childInterface;

                    return currentInterface == parent;
                });
        }

        private static Type ResolveGenericTypeDefinition(Type parent)
        {
            var shouldUseGenericType =
                !(parent.IsGenericType && parent.GetGenericTypeDefinition() != parent);

            if (parent.IsGenericType && shouldUseGenericType)
                parent = parent.GetGenericTypeDefinition();
            return parent;
        }

        public static bool IsNullable(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /* ====== Closing an open generic type ====== */

        /*
         * Note: see the discussion at https://stackoverflow.com/q/4864496/886
         * - note especially Jon Skeet's discussion in the comments. The
         * approach I'm adopting here is a hybrid one: to look for the most
         * common failure conditions and to use Pokémon exception handling,
         * ugly as it may be, to mop up the complex cases.
         */

        /// <summary>
        ///  Try to close an open generic type, returning false on failure.
        ///  Throws exceptions in more complicated cases.
        /// </summary>
        /// <param name="openGeneric">
        ///  The open generic type to
        /// </param>
        /// <param name="typeParameters"></param>
        /// <param name="pokémon">
        ///  true to swallow exceptions and return null in complex cases; false
        ///  to allow the exceptions to propagate.
        /// </param>
        /// <param name="result"></param>
        /// <returns></returns>

        public static bool TryMakeGenericType
            (this Type openGeneric, Type[] typeParameters, bool pokémon, out Type result)
        {
            /*
             * First, check that we are starting with an open generic
             */
            if (!openGeneric.IsGenericTypeDefinition) {
                result = null;
                return false;
            }

            /*
             * Next, check that the number of type parameters matches
             */
            var genericArguments = openGeneric.GetGenericArguments();
            if (genericArguments.Length != typeParameters.Length) {
                result = null;
                return false;
            }

            /*
             * Next, check the constraints
             */
            for (var i = 0; i < typeParameters.Length; i++) {
                var genericArgument = genericArguments[i];
                var typeParameter = typeParameters[i];
                var constraints = genericArgument.GetGenericParameterConstraints();
                var attributes = genericArgument.GenericParameterAttributes;
                if (attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint)
                    && typeParameter.IsValueType) {
                    result = null;
                    return false;
                }

                if (attributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint)
                    && typeParameter.GetConstructor(Type.EmptyTypes) == null
                    && !typeParameter.IsValueType) {
                    result = null;
                    return false;
                }
                foreach (var constraint in constraints) {
                    if (!constraint.IsAssignableFrom(typeParameter)) {
                        result = null;
                        return false;
                    }
                }
            }

            if (pokémon)
            {
                try {
                    result = openGeneric.MakeGenericType(typeParameters);
                    return true;
                }
                catch {
                    result = null;
                    return false;
                }
            }
            else {
                result = openGeneric.MakeGenericType(typeParameters);
                return true;
            }
        }

        /// <summary>
        ///  Try to close an open generic type, returning false on failure.
        ///  Catches exceptions if it gets tripped up by more complicated cases.
        /// </summary>
        /// <param name="openGeneric"></param>
        /// <param name="typeParameters"></param>
        /// <param name="result"></param>
        /// <returns></returns>

        public static bool TryMakeGenericType
            (this Type openGeneric, Type[] typeParameters, out Type result)
        {
            return TryMakeGenericType(openGeneric, typeParameters, true, out result);
        }
    }
}
