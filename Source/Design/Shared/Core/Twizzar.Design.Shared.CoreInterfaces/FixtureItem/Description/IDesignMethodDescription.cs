using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Method description interface for design domain.
    /// </summary>
    public interface IDesignMethodDescription : IMethodDescription
    {
        /// <summary>
        /// Gets the parameter types as a string.
        /// </summary>
        string FriendlyParameterTypes { get; }
    }
}