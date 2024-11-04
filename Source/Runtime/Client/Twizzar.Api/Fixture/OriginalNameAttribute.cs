using System;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Fixture;

/// <summary>
/// Attribute to annotate member paths with their original name.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
[ExcludeFromCodeCoverage]
public class OriginalNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OriginalNameAttribute"/> class.
    /// </summary>
    /// <param name="memberName"></param>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used for analyzing.")]
    public OriginalNameAttribute(string memberName)
    {
    }
}