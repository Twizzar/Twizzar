using System.Collections.Generic;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;

/// <summary>
/// A Member configuration.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract record MemberConfiguration<T> : IMemberConfiguration
    where T : MemberConfiguration<T>
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberConfiguration{T}"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="source"></param>
    protected MemberConfiguration(string name, IConfigurationSource source)
    {
        EnsureHelper.GetDefault.Parameter(name, nameof(name))
            .IsNotNullAndNotEmpty()
            .ThrowOnFailure();

        EnsureHelper.GetDefault.Parameter(source, nameof(source))
            .ThrowWhenNull();

        this.Name = name;
        this.MemberPathName = name;
        this.Source = source;
    }

    #endregion

    #region properties

    /// <inheritdoc/>
    public string Name { get; init; }

    /// <inheritdoc />
    public string MemberPathName { get; init; }

    /// <inheritdoc/>
    public IConfigurationSource Source { get; init; }

    #endregion

    #region members

    /// <inheritdoc />
    public virtual IMemberConfiguration WithSource(IConfigurationSource source) =>
        this with { Source = source };

    /// <inheritdoc />
    public IMemberConfiguration WithName(string name) =>
        this with { Name = name };

    /// <inheritdoc />
    public bool Equals(IMemberConfiguration other, bool ignoreSource)
    {
        if (other is not T otherMemberConfiguration)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Name == other.Name &&
               (ignoreSource || this.Source.Equals(other.Source)) &&
               this.GetAdditionalEqualityComponents()
                   .SequenceEqual(otherMemberConfiguration.GetAdditionalEqualityComponents());
    }

    /// <inheritdoc />
    public virtual bool Equals(MemberConfiguration<T> other) =>
        this.Equals(other, false);

    /// <inheritdoc />
    public int GetHashCode(bool ignoreSource)
    {
        var components = this.GetAdditionalEqualityComponents().Prepend(this.Name);

        if (!ignoreSource)
        {
            components = components.Prepend(this.Source);
        }

        return HashEqualityComparer.CombineHashes(components.GetHashCodeOfElements());
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
        this.GetHashCode(false);

    /// <summary>
    /// Get addition equality components. Do not include Name or Source.
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<object> GetAdditionalEqualityComponents();

    #endregion
}