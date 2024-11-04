using System;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Fixture;

/// <summary>
/// Attribute to define the method under test on a test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[ExcludeFromCodeCoverage]
public class TestSourceAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestSourceAttribute"/> class.
    /// </summary>
    /// <param name="memberName">Provide the member name with the <c>nameof</c> keyword.
    /// <example>
    /// [TestSource(nameof(MyClass.MyMethod)))
    /// </example>
    /// </param>
    public TestSourceAttribute(string memberName)
    {
    }
}