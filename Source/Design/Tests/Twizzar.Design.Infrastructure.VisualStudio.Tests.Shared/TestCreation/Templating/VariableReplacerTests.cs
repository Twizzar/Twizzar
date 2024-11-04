using System.Collections.Generic;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Templating;

[TestFixture]
public class VariableReplacerTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<VariableReplacer>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Variables_get_replaced()
    {
        // arrange
        var sut = new ItemBuilder<VariableReplacer>()
            .Build();

        IEnumerator<TemplateVariable> GetEnumerator()
        {
            yield return new TemplateVariable("a", "A");
        }

        var enumerator = GetEnumerator();

        var provider = Mock.Of<IVariablesProvider>(variablesProvider =>
            variablesProvider.GetEnumerator() == enumerator);

        // act
        var replacedCode = sut.ReplaceAll("$a$", provider);

        // assert
        replacedCode.Should().Be("A");
    }
}