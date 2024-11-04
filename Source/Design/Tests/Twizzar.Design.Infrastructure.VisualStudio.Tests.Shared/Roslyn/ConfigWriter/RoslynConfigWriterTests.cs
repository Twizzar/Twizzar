using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using NUnit.Framework;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn.ConfigWriter;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog.Logging;
using Twizzar.TestCommon;

using TwizzarInternal.Fixture;

using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn.ConfigWriter;

[TestFixture]
public partial class RoslynConfigWriterTests
{
    #region static fields and constants

    private const string Code = @"
using Twizzar.Config.Fixture;
using Twizzar.Config;
using System.Collections;

namespace TestNamespace
{
    public partial class MyConfig : ItemConfig<TestClass>
    {
        <<ConfigStatements>>
    }

    pulbic class TestClass
    {
        public int IntProperty { get; set; }
        public int _intField;
        public IList ListProperty { get; set; }
        
        public TestClass(int intParam, string stringParam) { }

        public int Method() {}
        public string Method(string s) {}
    }
}
";

    #endregion

    #region fields

    private const string RootItemPath = "TestNamespace.TestClass";

    private Workspace _workspace;
    private IBuildInvocationSpanQuery _invocationSpanQuery;
    private IDocumentFileNameQuery _documentFileNameQuery;
    private IRoslynContextQuery _roslynContextQuery;
    private IRoslynConfigFinder _roslynConfigFinder;

    #endregion

    #region properties

    private FixtureItemId FixtureItemId =>
        FixtureItemId.CreateNamed(
                RootItemPath,
                TypeFullName.Create(RootItemPath))
            .WithRootItemPath(RootItemPath);

    #endregion

    #region members

    [SetUp]
    public async Task SetUp()
    {
        LoggerFactory.SetConfig(new LoggerConfigurationBuilder());

        this._invocationSpanQuery = new ItemBuilder<IBuildInvocationSpanQuery>()
            .With(p => p.GetSpanAsync.Value(Result.SuccessAsync<IViSpan, Failure>(Mock.Of<IViSpan>())))
            .Build();

        this._documentFileNameQuery = new ItemBuilder<IDocumentFileNameQuery>()
            .With(p => p.GetDocumentFileName.Value(Result.SuccessAsync<string, Failure>("")))
            .Build();

        await this.SetupWorkspaceAsync(Code);
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<RoslynConfigEventWriter>()
            .SetupParameter("workspace", new RoslynWorkspaceBuilder().Build())
            .IgnoreParameter("mainSynchronizationContext")
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task PropertyTest()
    {
        // arrange
        var sut = this.CreateSut();
        var config = new FiveValueMemberConfigurationBuilder("IntProperty").Build();

        //act
        await sut.UpdateConfigAsync(this.FixtureItemId, config);

        // assert
        var classNode = await this.GetClassNodeAsync();
        Console.WriteLine(classNode);

        var statementSyntax = classNode.DescendantNodes().OfType<InvocationExpressionSyntax>().First();

        ValidateStatement(statementSyntax, "this.With(p => p.IntProperty.Value(5))");
    }

    [Test]
    public async Task FieldTest()
    {
        // arrange
        var sut = this.CreateSut();

        var config = new FiveValueMemberConfigurationBuilder("_intField").Build();

        // act
        await sut.UpdateConfigAsync(this.FixtureItemId, config);

        // assert
        var classNode = await this.GetClassNodeAsync();
        Console.WriteLine(classNode);

        var statementSyntax = classNode.DescendantNodes().OfType<InvocationExpressionSyntax>().First();

        ValidateStatement(statementSyntax, "this.With(p => p._intField.Value(5))");
    }

    [Test]
    public async Task MethodTest()
    {
        // arrange
        var sut = this.CreateSut();

        var memberName = "Method";
        var name = $"{memberName}__String";

        var source = Build.New<IConfigurationSource>();

        var memberConfig = MethodConfiguration.Create(
            name,
            memberName,
            source,
            new ValueMemberConfiguration("a", 5, source),
            typeof(string),
            nameof(String));

        // act
        await sut.UpdateConfigAsync(this.FixtureItemId, memberConfig);

        // assert
        var classNode = await this.GetClassNodeAsync();
        Console.WriteLine(classNode);

        var statementSyntax = classNode.DescendantNodes().OfType<InvocationExpressionSyntax>().First();

        ValidateStatement(statementSyntax, "this.With(p => p.Method__String.Value(5))");
    }

    [Test]
    public async Task CtorTest()
    {
        // arrange
        var sut = this.CreateSut();

        var memberName = "intParam";

        var source = Build.New<IConfigurationSource>();

        var memberConfig = CtorMemberConfiguration.Create(
            source,
            new ValueMemberConfiguration(memberName, 5, source));

        // act
        await sut.UpdateConfigAsync(this.FixtureItemId, memberConfig);

        // assert
        var classNode = await this.GetClassNodeAsync();
        Console.WriteLine(classNode);

        var statementSyntax = classNode.DescendantNodes().OfType<InvocationExpressionSyntax>().First();

        ValidateStatement(statementSyntax, "this.With(p => p.Ctor.intParam.Value(5))");
    }

    [TestCase(typeof(IList), "Stub<IList>")]
    [TestCase(typeof(Array), "InstanceOf<Array>")]
    public async Task LinkTest(Type memberType, string valueConfig)
    {
        // arrange
        var sut = this.CreateSut();

        var memberName = "ListProperty";

        var source = Mock.Of<IConfigurationSource>();

        var memberConfig = new LinkMemberConfiguration(
            memberName,
            FixtureItemId.CreateNameless(
                TypeFullName.CreateFromType(memberType)),
            source);

        // act
        await sut.UpdateConfigAsync(this.FixtureItemId, memberConfig);

        // assert
        var classNode = await this.GetClassNodeAsync();
        Console.WriteLine(classNode);

        var statementSyntax = classNode.DescendantNodes().OfType<InvocationExpressionSyntax>().First();

        ValidateStatement(statementSyntax, $"this.With(p => p.ListProperty.{valueConfig}())");
    }

    [Test]
    public async Task WhenSameMemberPathIsWrittenTwiceUpdateTheEntry()
    {
        // arrange
        var sut = this.CreateSut();

        var memberConfig = new FiveValueMemberConfigurationBuilder("IntProperty").Build();

        // act
        await sut.UpdateConfigAsync(this.FixtureItemId, memberConfig);

        // assert
        var classNode = await this.GetClassNodeAsync();
        Console.WriteLine(classNode);

        var statements =
            classNode
                .ChildNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .Single()
                .Body.Statements;

        statements.Should().HaveCount(1);
        var statementSyntax = statements.First();
        ValidateStatement(statementSyntax, "this.With(p => p.IntProperty.Value(5));");
    }

    private RoslynConfigWriter CreateSut() =>
        new ItemBuilder<RoslynConfigWriter>()
            .With(p => p.Ctor.buildInvocationSpanQuery.Value(this._invocationSpanQuery))
            .With(p => p.Ctor.documentFileNameQuery.Value(this._documentFileNameQuery))
            .With(p => p.Ctor.workspace.Value(this._workspace))
            .With(p => p.Ctor.roslynContextQuery.Value(this._roslynContextQuery))
            .With(p => p.Ctor.roslynConfigFinder.Value(this._roslynConfigFinder))
            .With(p => p.Ctor.typeSymbolQuery.Value(new TypeSymbolQuery()))
            .Build();

    private async Task SetupWorkspaceAsync(
        string code,
        string configurationStatements = "",
        Func<Workspace, Document, Task> initWorkspaceAction = null)
    {
        var filePath = "testPath.cs";

        if (configurationStatements != null)
        {
            configurationStatements = @$"
public MyConfig()
{{
        {configurationStatements}
}}";
        }

        var newCode = code.Replace("<<ConfigStatements>>", configurationStatements);

        this._workspace = new RoslynWorkspaceBuilder()
            .AddDocument(filePath, newCode)
            .AddReference(typeof(int).Assembly.Location)
            .Build();

        var document = this._workspace.CurrentSolution.Projects.First().Documents.Last();
        var compilation = await this._workspace.CurrentSolution.Projects.First().GetCompilationAsync();

        if (initWorkspaceAction != null)
        {
            await initWorkspaceAction.Invoke(this._workspace, document);
        }

        var semanticModel = await document.GetSemanticModelAsync();
        var root = await document.GetSyntaxRootAsync();
        var classNode = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        var itemConfig = ((ITypeSymbol)ModelExtensions.GetDeclaredSymbol(semanticModel, classNode)).BaseType;

        var builderInfo = new BuilderInformation(
            Mock.Of<ITypeSymbol>(),
            itemConfig,
            classNode,
            ObjectCreationExpression(IdentifierName("Test")));

        this._roslynContextQuery = new ItemBuilder<IRoslynContextQuery>()
            .With(p => p.GetContextAsync.Value(
                Result.SuccessAsync<IRoslynContext, Failure>(
                    new RoslynContext(semanticModel, document, root, compilation))))
            .Build();

        this._roslynConfigFinder = new ItemBuilder<IRoslynConfigFinder>()
            .With(p => p.FindConfigClass.Value(Maybe.Some<IBuilderInformation>(builderInfo)))
            .Build();
    }

    private async Task<ClassDeclarationSyntax> GetClassNodeAsync()
    {
        var document = this._workspace.CurrentSolution.Projects.First().Documents.Last();
        var root = await document.GetSyntaxRootAsync();
        return root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
    }

    private static void ValidateStatement(SyntaxNode expressionSyntax, string expectedStatements)
    {
        expressionSyntax.ToString()
            .Should()
            .BeEquivalentTo(ParseStatement(expectedStatements).NormalizeWhitespace().ToString());
    }

    #endregion

    #region Nested type: FiveValueMemberConfigurationConfig

    private class FiveValueMemberConfigurationBuilder : ItemBuilder<ValueMemberConfiguration, FiveValueMemberConfigurationBuilderPaths>
    {
        public FiveValueMemberConfigurationBuilder(string memberName)
        {
            this.With(p => p.Ctor.value.Value(5));
            this.With(p => p.Ctor.name.Value(memberName));
        }
    }

    #endregion
}