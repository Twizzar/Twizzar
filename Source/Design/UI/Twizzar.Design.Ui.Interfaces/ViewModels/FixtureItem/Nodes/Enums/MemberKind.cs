namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Enums
{
    /// <summary>
    /// Identifies the kind of the fixture member.
    /// </summary>
    public enum MemberKind
    {
        /// <summary>
        /// Default value if the kind isn't identified yet.
        /// </summary>
        NotDefined = 0,

        /// <summary>
        /// For all property-members.
        /// </summary>
        Property = 10,

        /// <summary>
        /// For all field-members.
        /// </summary>
        Field = 20,

        /// <summary>
        /// For all method/ctor-members.
        /// </summary>
        Method = 30,
    }
}