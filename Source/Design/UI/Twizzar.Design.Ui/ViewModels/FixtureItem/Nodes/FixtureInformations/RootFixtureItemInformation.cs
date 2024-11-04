using System;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Enums;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations
{
    /// <summary>
    /// Fixture information for the root of a Fixture Item.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RootFixtureItemInformation : IFixtureItemInformation
    {
        #region ctors

        private RootFixtureItemInformation(FixtureItemId id, IMemberConfiguration memberConfiguration, string path)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(id, nameof(id))
                .Parameter(memberConfiguration, nameof(memberConfiguration))
                .ThrowWhenNull();

            EnsureHelper.GetDefault.Parameter(path, nameof(path)).IsNotNullAndNotEmpty().ThrowOnFailure();

            this.Id = id;
            this.MemberConfiguration = memberConfiguration;
            this.Path = path;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public FixtureItemId Id { get; }

        /// <inheritdoc />
        public string ParentPath => this.Path;

        /// <inheritdoc />
        public string Path { get; }

        /// <inheritdoc />
        public IBaseDescription FixtureDescription => throw new NotImplementedException();

        /// <inheritdoc />
        public IMemberConfiguration MemberConfiguration { get; }

        /// <inheritdoc />
        public ITypeFullName TypeFullName => this.Id.TypeFullName;

        /// <inheritdoc />
        public bool CanBeExpanded { get; private set; } = true;

        /// <inheritdoc />
        public MemberKind Kind => throw new NotImplementedException();

        /// <inheritdoc />
        public MemberModifier Modifier => throw new NotImplementedException();

        /// <inheritdoc />
        public string DisplayValue => throw new NotImplementedException();

        /// <inheritdoc />
        public string DisplayName => throw new NotImplementedException();

        /// <inheritdoc />
        public string FriendlyTypeFullName => throw new NotImplementedException();

        /// <inheritdoc />
        public string FriendlyTypeName => throw new NotImplementedException();

        /// <inheritdoc />
        public bool IsDefault => throw new NotImplementedException();

        #endregion

        #region members

        /// <summary>
        /// Create a new instance of <see cref="RootFixtureItemInformation"/>.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <returns>A new instance of <see cref="RootFixtureItemInformation"/>.</returns>
        public static RootFixtureItemInformation Create(FixtureItemId id) =>
            new(
                id,
                new LinkMemberConfiguration(
                    "root",
                    id,
                    id.RootItemPath.Match<IConfigurationSource>(
                        s => new FromBuilderClass(s),
                        () => new FromSystemDefault())),
                id.RootItemPath.SomeOrProvided(() => throw new InternalException($"{nameof(id.RootItemPath)} is not set.")));

        /// <inheritdoc />
        public IFixtureItemInformation With(IMemberConfiguration configuration) =>
            new RootFixtureItemInformation(this.Id, configuration, this.Path);

        /// <inheritdoc />
        public IFixtureItemInformation With(FixtureItemId id) =>
            new RootFixtureItemInformation(id, this.MemberConfiguration, this.Path);

        /// <inheritdoc />
        public IFixtureItemInformation WithNotExpandable() =>
            new RootFixtureItemInformation(this.Id, this.MemberConfiguration, this.Path) { CanBeExpanded = false };

        #endregion
    }
}