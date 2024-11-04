using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem
{
    /// <summary>
    /// Fixture Information which are important for the view models.
    /// </summary>
    public interface IFixtureItemInformation
    {
        #region properties

        /// <summary>
        /// Gets the fixture item id of the Fixture Item and not the member.
        /// </summary>
        FixtureItemId Id { get; }

        /// <summary>
        /// Gets the parent path.
        /// </summary>
        string ParentPath { get; }

        /// <summary>
        /// Gets the path to this member.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the description of the current node / member.
        /// </summary>
        IBaseDescription FixtureDescription { get; }

        /// <summary>
        /// Gets the member configuration.
        /// </summary>
        IMemberConfiguration MemberConfiguration { get; }

        /// <summary>
        /// Gets the type full name of the member.
        /// </summary>
        ITypeFullName TypeFullName { get; }

        /// <summary>
        /// Gets a value indicating whether the fixture can be expanded.
        /// </summary>
        bool CanBeExpanded { get; }

        /// <summary>
        /// Gets the member kind.
        /// </summary>
        MemberKind Kind { get; }

        /// <summary>
        /// Gets the member modifier.
        /// </summary>
        MemberModifier Modifier { get; }

        /// <summary>
        /// Gets the display value.
        /// </summary>
        string DisplayValue { get; }

        /// <summary>
        /// Gets the display name of the fixture.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the friendly display name.
        /// </summary>
        string FriendlyTypeFullName { get; }

        /// <summary>
        /// Gets the friendly display name of the type only.
        /// </summary>
        string FriendlyTypeName { get; }

        /// <summary>
        /// Gets a value indicating whether this is the default value.
        /// </summary>
        bool IsDefault { get; }

        #endregion

        #region members

        /// <summary>
        /// Create a new <see cref="IFixtureItemInformation"/> with a new member configuration.
        /// </summary>
        /// <param name="configuration">The member configuration.</param>
        /// <returns>A new instance of <see cref="IFixtureItemInformation"/>.</returns>
        IFixtureItemInformation With(IMemberConfiguration configuration);

        /// <summary>
        /// Create a new <see cref="IFixtureItemInformation"/> with a new fixture item id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A new instance of <see cref="IFixtureItemInformation"/>.</returns>
        public IFixtureItemInformation With(FixtureItemId id);

        /// <summary>
        /// Create a new <see cref="IFixtureItemInformation"/> which can not be expanded.
        /// </summary>
        /// <returns>A new instance of <see cref="IFixtureItemInformation"/>.</returns>
        public IFixtureItemInformation WithNotExpandable();

        #endregion
    }
}