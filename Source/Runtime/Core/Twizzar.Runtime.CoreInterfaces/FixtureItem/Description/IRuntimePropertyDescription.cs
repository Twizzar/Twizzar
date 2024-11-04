using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Discovers the attributes of a property and provides access to property metadata.
    /// </summary>
    public interface IRuntimePropertyDescription : IPropertyDescription, IRuntimeMemberDescription
    {
    }
}