using System.Collections.Generic;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.Util;

namespace Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Extension methods for the <see cref="ITypeDescription"/>.
    /// </summary>
    public static class TypeDescriptionExtensions
    {
        #region members

        /// <summary>
        /// Get all members: Constructors, methods, fields and properties.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>Sequence of all found members.</returns>
        public static IEnumerable<IMemberDescription> GetMembers(this ITypeDescription self) =>
            self.GetDeclaredConstructors().OfType<IMemberDescription>()
                .Concat(self.GetDeclaredMethods().OfType<IMemberDescription>())
                .Concat(self.GetDeclaredFields().OfType<IMemberDescription>())
                .Concat(self.GetDeclaredProperties().OfType<IMemberDescription>());

        /// <summary>
        /// Get all member names of a type.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetMemberNames(this ITypeDescription self) =>
            self.GetMembers().Select(description => description.Name);

        /// <summary>
        /// Get all modifiable properties. Which means:
        /// - The declaring type is and interface
        /// or
        /// - the property has a setter and
        /// - the property is an auto property.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>A sequence of <see cref="IDesignPropertyDescription"/>.</returns>
        public static IEnumerable<IDesignPropertyDescription> GetDeclaredModifiableProperties(
            this ITypeDescription self) =>
            self.GetDeclaredProperties()
                .OfType<IDesignPropertyDescription>()
                .Where(p => self.IsInterface || p.CanWrite);

        /// <summary>
        /// Get all modifiable fields. Which means:
        /// - The field is no backing field of a property
        /// or
        /// - the declaring type is a class or a structs and
        /// - the related Property has no setter.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>A sequence of <see cref="IDesignFieldDescription"/>.</returns>
        public static IEnumerable<IDesignFieldDescription> GetDeclaredModifiableFields(this ITypeDescription self) =>
            self.GetDeclaredFields()
                .OfType<IDesignFieldDescription>()
                .Where(
                    f => !f.IsBackingField ||
                         ((self.IsClass || self.IsStruct) && !f.BackingFieldProperty.GetValueUnsafe().CanWrite));

        /// <summary>
        /// Get all selectable methods. Which means:
        /// - the method is not a getter or setter form a property.
        /// - the declaring type is an interface.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>A sequence of <see cref="IDesignMethodDescription"/>.</returns>
        public static IEnumerable<IDesignMethodDescription> GetDeclaredTwizzarSelectableMethod(this ITypeDescription self) =>
            self.GetDeclaredMethods()
                .OfType<IDesignMethodDescription>()
                .Where(description => description.MethodKind != MethodKind.Property)
                .Where(_ => self.IsInterface);

        /// <summary>
        /// Gets distinct name and return type for given enumerable of <see cref="IMethodDescription"/>.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>Distinct sequence of <see cref="IMethodDescription"/>.</returns>
        public static IEnumerable<IMethodDescription> DistinctByNameAndReturnType(this IEnumerable<IMethodDescription> self) =>
            self.Distinct(new MethodComparer());

        private sealed class MethodComparer : IEqualityComparer<IMethodDescription>
        {
            public bool Equals(IMethodDescription x, IMethodDescription y) =>
                (x is null && y is null) ||
                (x?.Name == y?.Name && x!.TypeFullName.Equals(y!.TypeFullName));

            public int GetHashCode(IMethodDescription obj) =>
                HashEqualityComparer.CombineHashes(obj.Name.GetHashCode(), obj.TypeFullName.GetHashCode());
        }

        #endregion
    }
}