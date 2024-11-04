using System.Diagnostics.CodeAnalysis;

// ReSharper disable ConvertToStaticClass
#pragma warning disable S3453 // Classes should not have only "private" constructors

namespace Twizzar.Fixture;

/// <summary>
/// Marker class for declaring paths with the return type void.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class TzVoid
{
    #region ctors

    private TzVoid()
    {
    }

    #endregion
}