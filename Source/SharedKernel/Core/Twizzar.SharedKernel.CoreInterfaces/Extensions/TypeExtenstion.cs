using System;
using System.Collections.Generic;
using System.Reflection;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extenstion method for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtenstion
    {
        /// <summary>
        /// Determines whether this type can be null.
        /// </summary>
        /// <param name="self">The type.</param>
        /// <returns>
        ///   <c>true</c> if this type can be null; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanBeNull(this Type self) =>
            !self.IsValueType || (Nullable.GetUnderlyingType(self) != null);

        /// <summary>
        /// Checks whether the given type is a nullable type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if this type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(this Type type) =>
            type.GetTypeInfo().IsGenericType && (object)type.GetGenericTypeDefinition() == (object)typeof(Nullable<>);

        /// <summary>
        /// Checks whether the given type is a struct.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        ///   <c>true</c> if this type is a struct; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsStruct(this Type type) =>
            type.IsValueType && !type.IsEnum && type.Name != typeof(void).Name;

        /// <summary>
        /// Determines the array dimension.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Gets the array dimension or empty enumerable.</returns>
        public static IEnumerable<int> GetArrayDimension(this Type type)
        {
            if (!type.IsArray)
            {
                yield break;
            }

            var arrayElementType = type.GetElementType();

            if (arrayElementType?.IsArray == true)
            {
                foreach (var value in GetArrayDimension(arrayElementType))
                {
                    yield return value;
                }
            }

            yield return type.GetArrayRank();
        }

        /// <summary>
        /// Gets a value that indicates whether the current Type represents a type parameter in the definition of a generic method.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsGenericMethodParameter(this Type type) =>
            type.IsGenericParameter && type.DeclaringMethod != null;

        /// <summary>
        /// Determines whether the current type can be assigned to a variable of the specified targetType.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static bool IsAssignableTo(this Type self, Type targetType) =>
            targetType?.IsAssignableFrom(self) ?? false;
    }
}