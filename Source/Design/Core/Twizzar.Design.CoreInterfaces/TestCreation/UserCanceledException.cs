using System;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Exception thrown when the user canceled the operation.
/// </summary>
[ExcludeFromCodeCoverage]
public class UserCanceledException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCanceledException"/> class.
    /// </summary>
    /// <param name="message"></param>
    public UserCanceledException(string message)
        : base(message)
    {
    }
}