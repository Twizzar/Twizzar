namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration
{
    /// <summary>
    /// Enum to determine how the ctor will be selected.
    /// </summary>
    public enum CtorSelectionBehavior
    {
        /// <summary>
        /// Ctor selector for max parameters.
        /// </summary>
        Max,

        /// <summary>
        /// Ctor selector for min parameters.
        /// </summary>
        Min,

        /// <summary>
        /// Custom Ctor selector.
        /// </summary>
        Custom,
    }
}
