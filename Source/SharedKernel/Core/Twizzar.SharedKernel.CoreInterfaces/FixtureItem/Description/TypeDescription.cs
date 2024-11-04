using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional.Monads.MaybeMonad;

#pragma warning disable CS1591, S107, SA1600

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// ConcreteType Description value object.
    /// </summary>
    public abstract class TypeDescription : BaseTypeDescription, ITypeDescription
    {
        #region ctors

        protected TypeDescription(IBaseTypeService baseTypeService)
            : base(baseTypeService)
        {
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public Maybe<ITypeFullName> BaseType { get; protected set; }

        /// <inheritdoc />
        public ImmutableArray<string> ImplementedInterfaceNames { get; protected set; }

        /// <inheritdoc />
        public bool IsDelegate { get; protected set; }

        /// <inheritdoc />
        public bool IsAbstract { get; protected set; }

        /// <inheritdoc />
        public bool IsNested { get; protected set; }

        /// <inheritdoc />
        public bool IsSealed { get; protected set; }

        /// <inheritdoc />
        public bool IsStatic { get; protected set; }

        /// <inheritdoc />
        public bool IsGeneric { get; protected set; }

        /// <inheritdoc />
        public bool IsInheritedFromICollection =>
            this.ImplementedInterfaceNames.Contains(
                typeof(ICollection).FullName);

        /// <inheritdoc />
        public IImmutableDictionary<int, GenericParameterType> GenericTypeArguments { get; protected set; }

        /// <inheritdoc />
        public FixtureKind DefaultFixtureKind => this.GetDefaultFixtureKind();

        #endregion

        #region members

        /// <inheritdoc />
        public abstract ImmutableArray<IMethodDescription> GetDeclaredConstructors();

        /// <inheritdoc />
        public abstract ImmutableArray<IFieldDescription> GetDeclaredFields();

        /// <inheritdoc />
        public abstract ImmutableArray<IMethodDescription> GetDeclaredMethods();

        /// <inheritdoc />
        public abstract ImmutableArray<IPropertyDescription> GetDeclaredProperties();

        /// <inheritdoc />
        public abstract ITypeDescription GetGenericTypeArgumentDescription(int position);

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.TypeFullName;
        }

        protected static ImmutableArray<T> Merge<T>(ImmutableArray<T> a, ImmutableArray<T> b)
            where T : IMemberDescription
        {
            var builder = a.ToBuilder();
            var set = a.Select(description => description.Name).ToImmutableHashSet();

            foreach (var field in b.Where(field => !set.Contains(field.Name)))
            {
                builder.Add(field);
            }

            return builder.ToImmutable();
        }

        private FixtureKind GetDefaultFixtureKind() =>
            (this.IsBaseType || this.IsNullableBaseType) switch
            {
                true => FixtureKind.BaseType,
                false => this switch
                {
                    { IsInterface: true } => FixtureKind.Mock,

                    // use fixtureKind class for class, struct and array
                    { IsClass: true } => FixtureKind.Class,
                    { IsStruct: true } => FixtureKind.Class,
                    { IsArray: true } => FixtureKind.Class,

                    // handle enums as kind baseType
                    { IsEnum: true } => FixtureKind.BaseType,

                    // This state should not be possible the type is always a interface or a class.
                    _ => throw new ResolveTypeDescriptionException(
                        $"{nameof(TypeDescription)} of type {this.TypeFullName} is not an interface and not an class."),
                },
            };

        #endregion
    }
}