using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Query.Services.ReadModel
{
    /// <inheritdoc cref="IFixtureItemModel" />
    [ExcludeFromCodeCoverage]
    public record ObjectFixtureItemModel : IFixtureItemModel
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectFixtureItemModel"/> class.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <param name="fixtureConfigurations"><see cref="IFixtureConfiguration"/> used for general fixture item configurations.</param>
        /// <param name="usedConstructor">The configured used constructor.</param>
        /// <param name="properties">Properties.</param>
        /// <param name="fields">Fields.</param>
        /// <param name="methods">Methods.</param>
        /// <param name="declaredConstructors">All declared constructor of the type.</param>
        /// <param name="typeDescription">The type description.</param>
        public ObjectFixtureItemModel(
            FixtureItemId id,
            IImmutableDictionary<string, IFixtureConfiguration> fixtureConfigurations,
            Maybe<FixtureItemConstructorModel> usedConstructor,
            IImmutableDictionary<string, FixtureItemMemberModel> properties,
            IImmutableDictionary<string, FixtureItemMemberModel> fields,
            IImmutableDictionary<string, FixtureItemMemberModel> methods,
            ImmutableArray<IMethodDescription> declaredConstructors,
            ITypeDescription typeDescription)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(properties, nameof(properties))
                .Parameter(fields, nameof(fields))
                .Parameter(methods, nameof(methods))
                .Parameter(fixtureConfigurations, nameof(fixtureConfigurations))
                .Parameter(id, nameof(id))
                .Parameter(typeDescription, nameof(typeDescription))
                .ThrowWhenNull();

            this.UsedConstructor = usedConstructor;
            this.Properties = properties;
            this.Fields = fields;
            this.Methods = methods;
            this.DeclaredConstructors = declaredConstructors;
            this.FixtureConfigurations = fixtureConfigurations;
            this.Id = id;
            this.Description = typeDescription;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public FixtureItemId Id { get; init; }

        /// <inheritdoc />
        public IImmutableDictionary<string, IFixtureConfiguration> FixtureConfigurations { get; init; }

        /// <inheritdoc />
        public ITypeDescription Description { get; init; }

        /// <summary>
        /// Gets The used configured constructor. Some when kind is <see cref="FixtureKind.Class"/> else None.
        /// </summary>
        public Maybe<FixtureItemConstructorModel> UsedConstructor { get; init; }

        /// <summary>
        /// Gets all properties of the type.
        /// </summary>
        public IImmutableDictionary<string, FixtureItemMemberModel> Properties { get; init; }

        /// <summary>
        /// Gets all fields of the type.
        /// </summary>
        public IImmutableDictionary<string, FixtureItemMemberModel> Fields { get; init; }

        /// <summary>
        /// Gets all methods of the type.
        /// </summary>
        public IImmutableDictionary<string, FixtureItemMemberModel> Methods { get; init; }

        /// <summary>
        /// Gets all declared constructors of the type.
        /// </summary>
        public ImmutableArray<IMethodDescription> DeclaredConstructors { get; init; }

        #endregion
    }
}