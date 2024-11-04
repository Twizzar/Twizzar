using System.Linq;
using System.Threading;
using FluentAssertions;
using Microsoft.CodeAnalysis;

using Moq;

using NUnit.Framework;
using Twizzar.Analyzer;
using Twizzar.Analyzer.SourceTextGenerators;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.TestCommon;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Analyzer2019.App.Tests.Twizzar.Analyzer.App.SourceTextGenerators
{
    [TestFixture]
    public class BuilderExtensionMethodSourceTextGeneratorTests
    {
        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            TwizzarInternal.Fixture.Verify.Ctor<BuilderExtensionMethodSourceTextGenerator>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void Class_is_generated_correctly()
        {
            var sut = new BuilderExtensionMethodSourceTextGenerator();
            var cancellationTokenSource = new CancellationTokenSource();

            var nsSymbol = Mock.Of<INamespaceSymbol>(
                namespaceSymbol =>
                    namespaceSymbol.IsGlobalNamespace == true);

            var symbol = Mock.Of<ITypeSymbol>(typeSymbol => 
                typeSymbol.MetadataName == "MyType" &&
                typeSymbol.Name == "MyType" &&
                typeSymbol.ContainingNamespace == nsSymbol);

            var information = new ItemBuilderCreationInformation(
                "",
                null,
                symbol,
                "MyPathProvider");

            var generateClass = sut.GenerateClass(
                new[] {information},
                cancellationTokenSource.Token);

            var code = TestHelper.AssertAndUnwrapSuccess(generateClass);
            code.Should().Contain("namespace Twizzar.Fixture");
            code.Should().Contain("this ItemBuilder<MyType> builder");
            code.Should().Contain("Func<MyPathProvider, MemberConfig<MyType>> func");

            // TODO verify Scope ext
        }

        [Test]
        public void When_cancel_requested_returns_OperationCanceledFailure()
        {
            var sut = new BuilderExtensionMethodSourceTextGenerator();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            var information = new ItemBuilderCreationInformation(
                "",
                null,
                null,
                "");

            var result = sut.GenerateClass(
                Enumerable.Empty<ItemBuilderCreationInformation>().Append(information), 
                cancellationTokenSource.Token);

            result.IsFailure.Should().BeTrue();
            result.AsResultValue()
                .Should()
                .BeAssignableTo<FailureValue<OperationCanceledFailure>>();
        }
    }
}