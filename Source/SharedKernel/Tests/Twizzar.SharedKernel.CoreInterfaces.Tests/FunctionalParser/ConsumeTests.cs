using FluentAssertions;
using NUnit.Framework;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Tests.FunctionalParser;

[TestFixture]
public class ConsumeTests
{
    [Test]
    public void AnyChar_consumes_any_char()
    {
        foreach (var c in new ItemBuilder<char>().BuildMany(50))
        {
            var result = Consume.AnyChar.Parse(c.ToString());
            var successValue = result.AsResultValue().Should().BeAssignableTo<SuccessValue<IParseSuccess<char>>>().Subject;
            successValue.Value.OutputPoint.Position.Should().Be(1);
            successValue.Value.ParsedSpan.Length.Should().Be(1);
            successValue.Value.ParsedSpan.Start.Position.Should().Be(0);
            successValue.Value.Value.Should().Be(c);
        }
    }
}