namespace Twizzar.SharedKernel.CoreInterfaces.Util
{
    /// <summary>
    /// Represents an object which can be merged.
    /// </summary>
    public interface IMergeable
    {
        /// <summary>
        /// Merge b into a.
        /// </summary>
        /// <param name="b"></param>
        /// <returns>A new merged object.</returns>
        object Merge(object b);
    }
}