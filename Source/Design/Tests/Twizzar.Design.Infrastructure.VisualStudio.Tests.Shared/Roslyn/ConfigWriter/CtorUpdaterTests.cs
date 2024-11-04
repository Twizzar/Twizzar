using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn.ConfigWriter;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;
using TwizzarInternal.Fixture;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn.ConfigWriter;

[TestFixture]
public class CtorUpdaterTests
{
    private class CtorUpdaterBuilder : ItemBuilder<CtorUpdater, CtorUpdaterBuilderCustomPaths>
    {
        private SyntaxTree Tree { get; init; }

        public CtorUpdaterBuilder(string code = null)
        {
            code ??= "class MyCtor{ public MyCtor() {}}";

            this.Tree = SyntaxFactory.ParseSyntaxTree(code);
            var ctorNode = this.Tree.GetRoot().DescendantNodes().OfType<ConstructorDeclarationSyntax>().Single();
            this.With(p => p.Ctor.ctorNode.Value(ctorNode));

            var compilation = CSharpCompilation.Create("TestAssembly", new[] { this.Tree });

            var context = Mock.Of<IRoslynContext>(
                roslynContext =>
                    roslynContext.Compilation == compilation);

            this.With(p => p.Ctor.context.Value(context));
        }

        public CtorUpdaterBuilder AddDiscovering(Func<SyntaxTree, IEnumerable<IdentifierNameSyntax>> nameSyntaxFunc)
        {
            var info = new PathProviderInformation(
                RandomString(),
                RandomString(),
                Mock.Of<ITypeSymbol>(),
                Mock.Of<ITypeSymbol>(),
                Mock.Of<SemanticModel>());

            var discoverer = new Mock<IDiscoverer>();

            foreach (var nameSyntax in nameSyntaxFunc(this.Tree))
            {
                var returnOperation =
                    new Mock<ISimpleValuesOperation<(IdentifierNameSyntax, SemanticModel, PathProviderInformation)>>();

                returnOperation
                    .Setup(operation => operation.Evaluate())
                    .Returns(new[] { (nameSyntax, info.SemanticModel, info) });

                discoverer
                    .Setup(
                        x =>
                            x.DiscoverMemberSelection(
                                It.Is<IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)>>(
                                    operation => ContainsNameSyntax(operation, nameSyntax))))
                    .Returns(returnOperation.Object);
            }

            this.With(p => p.Ctor.discoverer.Value(discoverer.Object));
            return this;
        }
    }

    private static bool ContainsNameSyntax(
        IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> operation,
        IdentifierNameSyntax nameSyntax) =>
        operation is ISimpleValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> so &&
        so.Evaluate().Any(tuple => tuple.Node == nameSyntax);

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<CtorUpdater>()
            .SetupParameter("ctorNode", SyntaxFactory.ConstructorDeclaration("MyCtor"))
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Statement_gets_inserted_correctly()
    {
        var sut = new CtorUpdaterBuilder().Build();
        var exceptedStatementString = "var a = 5;";
        var statement = new AddConfigurationMemberEdit(SyntaxFactory.ParseStatement(exceptedStatementString), "Path");

        sut.Update(Enumerable.Empty<IConfigurationMemberEdit>().Append(statement));

        var statementSyntax = sut.UpdatedCtorNode.Body.Statements;
        statementSyntax.Should().HaveCount(1);
        statementSyntax.First().ToString().Should().Be(exceptedStatementString);
    }

    [Test]
    public void Statement_gets_updated_correctly()
    {
        var exceptedStatementString = "var a = 8;";
        var statement = new AddConfigurationMemberEdit(SyntaxFactory.ParseStatement(exceptedStatementString), "Path");

        var code = "class MyClass : ItemBuilder<int, MyPath> { public MyClass() { this.With(p => p.Path); }}";

        var sut = new CtorUpdaterBuilder(code)
            .AddDiscovering(tree => tree
                .GetRoot()
                .DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(syntax => syntax.Identifier.Text == "p"))
            .Build();

        sut.Update(Enumerable.Empty<IConfigurationMemberEdit>().Append(statement));

        var statementSyntaxes = sut.UpdatedCtorNode.Body.Statements;
        statementSyntaxes.Should().HaveCount(1);
        statementSyntaxes.First().ToString().Should().Be(exceptedStatementString);
    }


    [Test]
    public void Statements_get_removed_correctly()
    {
        // arrange
        var code = @"
class MyClass
{
    public MyClass()
    {
        this.With(p => p.RootPath.A);
        this.With(p => p.RootPath.A.B);
    }
}";

        var sut = new CtorUpdaterBuilder(code)
            .AddDiscovering(tree => tree
                .GetRoot()
                .DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(syntax => syntax.Identifier.Text == "p"))
            .Build();

        // act
        sut.RemovePathWithChildren("RootPath.");

        // assert
        var statementSyntaxes = sut.UpdatedCtorNode.Body.Statements;
        statementSyntaxes.Should().HaveCount(0);
    }
}