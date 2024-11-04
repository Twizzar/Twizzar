using System.Collections.Generic;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;

/// <summary>
/// Equality comparer for <see cref="IMemberConfiguration"/>.
/// </summary>
public class MemberConfigurationEqualityComparer : IEqualityComparer<IMemberConfiguration>
{
    private readonly bool _ignoreSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberConfigurationEqualityComparer"/> class.
    /// </summary>
    /// <param name="ignoreSource"></param>
    public MemberConfigurationEqualityComparer(bool ignoreSource)
    {
        this._ignoreSource = ignoreSource;
    }

    /// <inheritdoc/>
    public bool Equals(IMemberConfiguration x, IMemberConfiguration y) =>
        x.Equals(y, this._ignoreSource);

    /// <inheritdoc/>
    public int GetHashCode(IMemberConfiguration obj) =>
        obj.GetHashCode(this._ignoreSource);
}