namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Enums
{
    /// <summary>
    /// Identifies the modifier of the fixture member.
    /// </summary>
    public enum MemberModifier
    {
        /// <summary>
        /// Default value if the modifier isn't identified yet.
        /// </summary>
        NotDefined = 0,

        /// <summary>
        /// For all members that are set to internal-visibility.
        /// </summary>
        Internal = 10,

        /// <summary>
        /// For all members that are set to public-visibility.
        /// </summary>
        Public = 20,

        /// <summary>
        /// For all members that are set to protected-visibility.
        /// </summary>
        Protected = 30,

        /// <summary>
        /// For all members that are set to private-visibility.
        /// </summary>
        Private = 40,
    }
}