using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[TestFixture]
public partial class RoslynConfigReaderTests
{
    private const string FileName = "testFile.cs";

    private const string Code = @"
using System;
using Twizzar.Config.Fixture;

public class MyConfig1 : ItemConfig<int>
{
}

public class MyConfig2 : ItemConfig<int>
{
}

public class MyUnitTests
{
    public static void New<T>(MyConfig c) { }

    public void TestMethod()
    {
        <<Invokation>>
    }
}
";

    private (RoslynContextQuery, IBuildInvocationSpanQuery Object) SetupCode(string invocation)
    {
        var code = Code.Replace("<<Invokation>>", invocation);
        Console.WriteLine(code);

        var workspace = new RoslynWorkspaceBuilder()
            //.AddReference(typeof(Twizzar.Fixture).Assembly.Location)
            .AddReference(typeof(int).Assembly.Location)
            .AddDocument(FileName, code)
            .Build();

        var match = Regex.Match(code, @"New<int>\(.*\)");
            
        var span = Mock.Of<IViSpan>(
            viSpan =>
                viSpan.Start == match.Index && viSpan.Length == match.Length);

        var invocationSpanQuery = new Mock<IBuildInvocationSpanQuery>();

        invocationSpanQuery.Setup(query => query.GetSpanAsync(It.IsAny<Maybe<string>>()))
            .Returns(Result.SuccessAsync<IViSpan, Failure>(span));

        return (new RoslynContextQuery(workspace), invocationSpanQuery.Object);
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<RoslynConfigReader>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task No_MemberConfiguration_returns_empty_sequence()
    {
        // arrange
            
        var documentFileNameQuery = new ItemBuilder<IDocumentFileNameQuery>()
            .With(p => p.GetDocumentFileName.Value(Result.SuccessAsync<string, Failure>(FileName)))
            .Build();

        var roslynConfigurationReaderMock = new Mock<IRoslynConfigurationItemReader>();

        roslynConfigurationReaderMock
            .Setup(reader => reader.ReadConfigurationItemsAsync(It.IsAny<IBuilderInformation>(), CancellationToken.None))
            .Returns(
                Task.FromResult<IImmutableDictionary<FixtureItemId, IConfigurationItem>>(
                    ImmutableDictionary<FixtureItemId, IConfigurationItem>.Empty));

        var (contextQuery, invocationQuery) = this.SetupCode("");

        var sut = new ItemBuilder<RoslynConfigReader>()
            .With(p => p.Ctor.roslynConfigurationItemReader.Value(roslynConfigurationReaderMock.Object))
            .With(p => p.Ctor.documentFileNameQuery.Value(documentFileNameQuery))
            .With(p => p.Ctor.roslynContextQuery.Value(contextQuery))
            .With(p => p.Ctor.buildInvocationSpanQuery.Value(invocationQuery))
            .Build();

        // act
        var result = await sut.GetAllAsync(TestHelper.RandomString(), CancellationToken.None);

        // assert
        result.Should().HaveCount(0);
    }
}