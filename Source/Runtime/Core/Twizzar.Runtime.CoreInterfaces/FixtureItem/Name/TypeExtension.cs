using System;
using System.Collections.Generic;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Name
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Creates a type full name from given reflection type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The type full name representing the reflection type.</returns>
        public static TypeFullName ToTypeFullName(this Type type) =>
            TypeFullName.Create(type);

        /// <summary>
        /// Get all implemented interfaces and base types.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            // is there any base type?
            if (type == null)
            {
                yield break;
            }

            // return all implemented or inherited interfaces
            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }

        /// <summary>
        /// If the method is a generic type gets the generic type definition;
        /// else returns the provided type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The generic type definition or the provided type.</returns>
        public static Type GetGenericTypeDefinitionIfPossible(this Type type) =>
            type.IsGenericType ? type.GetGenericTypeDefinition() : type;
    }
}
