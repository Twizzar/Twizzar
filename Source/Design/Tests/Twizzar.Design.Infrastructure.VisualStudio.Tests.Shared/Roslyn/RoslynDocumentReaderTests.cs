using System;
using System.Linq;
using System.Threading;

using FluentAssertions;

using Microsoft.CodeAnalysis;

using Moq;

using NUnit.Framework;

using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn.DocumentReader;
using Twizzar.SharedKernel.NLog.Logging;

using TwizzarInternal.Fixture;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

public class RoslynDocumentReaderBuilder : ItemBuilder<RoslynDocumentReader, RoslynDocumentReaderBuilderPaths>
{
    public RoslynDocumentReaderBuilder()
    {
        this.With(p => p.Ctor.itemBuilderFinderFactory.Create.Stub<IItemBuilderFinder>());
    }

    public RoslynDocumentReaderBuilder AddItemBuilderInformation(params IItemBuilderInformation[] builderInformation)
    {
        var mock = new Mock<IItemBuilderFinder>();

        mock.Setup(finder => finder.FindBuilderInformation(It.IsAny<SyntaxNode>()))
            .Returns(() => builderInformation);

        this.With(p => p.Ctor.itemBuilderFinderFactory.Create.Value(mock.Object));

        return this;
    }
}

[TestFixture]
public class RoslynDocumentReaderTests
{
    #region members

    [SetUp]
    public void SetUp()
    {
        LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
    }

    [Test]
    public void Ctor_test()
    {
        // assert
        Verify.Ctor<RoslynDocumentReader>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void When_ItemBuilderFinder_returns_one_item_also_return_one_item()
    {
        var objectCreationExpression =
            ObjectCreationExpression(IdentifierName("MyClass"));

        var info = new ItemBuilderInformation(
            objectCreationExpression,
            Mock.Of<ITypeSymbol>(),
            false);

        var sut = new RoslynDocumentReaderBuilder()
            .AddItemBuilderInformation(info)
            .Build();

        var result = sut.GetAdornmentInformation(Mock.Of<IRoslynContext>());

        result.Should().HaveCount(1);
    }

    [Test]
    public void Getting_adornment_aborts_when_cancellation_is_requested()
    {
        // arrange
        var objectCreationExpression =
            ObjectCreationExpression(IdentifierName("MyClass"));

        var info = new ItemBuilderInformation(
            objectCreationExpression,
            Mock.Of<ITypeSymbol>(),
            false);

        var sut = new RoslynDocumentReaderBuilder()
            .AddItemBuilderInformation(info)
            .Build();

        // act
        Action action = () => _ = sut.GetAdornmentInformation(Mock.Of<IRoslynContext>(), (new CancellationToken(true)))
            .ToList();

        // assert
        action.Should().Throw<OperationCanceledException>();
    }

    #endregion
}