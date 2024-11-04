using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.CodeAnalysis;

using Moq;

using NUnit.Framework;

using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.TestCommon;
using Twizzar.Fixture;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog.Logging;
using Twizzar.TestCommon.Configuration.Builders;
using Twizzar.TestCommon.TypeDescription.Builders;

using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[TestFixture]
public class RoslynConfigurationItemReaderTests
{
    [SetUp]
    public void SetUp()
    {
        LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        TwizzarInternal.Fixture.Verify.Ctor<RoslynConfigurationItemReader>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task Ctor_of_CustomBuilder_gets_read_correctly()
    {
        var sourceCode = $@"
using Twizzar.Fixture;

public class MyBuilder : ItemBuilder<int, MyPaths>{{
    public MyBuilder(){{
        this.With(p => p.Member.Value(3));
    }}
}}

public class Ut{{
    public void Test(){{
        new MyBuilder();
    }}
 }}
";

        var filePath = "testPath";

        var workspace = new RoslynWorkspaceBuilder()
            .AddDocument(filePath, sourceCode)
            .AddReference(typeof(int).Assembly.Location)
            .AddReference(typeof(ItemBuilder<>).Assembly.Location)
            .Build();

        var typeDescription = new TypeDescriptionBuilder()
            .WithDeclaredProperties(
                new PropertyDescriptionBuilder()
                    .WithReturnType(new TypeDescriptionBuilder().AsBaseType().Build())
                    .WithBaseType(true)
                    .WithName("Member")
                    .Build())
            .Build();

        var roslynContextQuery = new RoslynContextQuery(workspace);

        var memberConfigurationFinder = new RoslynMemberConfigurationFinder(new Discoverer());
        var descriptionFactory = Mock.Of<IRoslynDescriptionFactory>(factory =>
            factory.CreateDescription(It.IsAny<ITypeSymbol>()) == typeDescription);

        var sut = new RoslynConfigurationItemReader(
            new ConfigurationItemFactoryBuilder().Build(),
            roslynContextQuery,
            memberConfigurationFinder,
            descriptionFactory);

        var context = await roslynContextQuery.GetContextAsync(filePath)
            .GetSuccessUnsafeAsync();

        var builderInformation = new RoslynConfigFinder().FindConfigClass(
                RegexSpan.CreateWithStringMatch(sourceCode, "new MyBuilder()"),
                context)
            .GetValueUnsafe();

        var readConfigurationItems = await sut.ReadConfigurationItemsAsync(builderInformation, CancellationToken.None);

        readConfigurationItems.Should().HaveCount(1);
        var pair = readConfigurationItems.First();
        pair.Value.MemberConfigurations.Should().HaveCount(1);
        var memberConfig = pair.Value.MemberConfigurations["Member"];
        var valueMemberConfiguration = memberConfig.Should().BeAssignableTo<ValueMemberConfiguration>().Subject;
        valueMemberConfiguration.Value.Should().Be(new SimpleLiteralValue("3"));
        valueMemberConfiguration.Name.Should().Be("Member");
        valueMemberConfiguration.Source.Should().BeAssignableTo<FromBuilderClass>();
    }

    [Test]
    public async Task Configuration_after_object_creation_get_read_correctly()
    {
        var sourceCode = $@"
using Twizzar.Fixture;

public class MyBuilder : ItemBuilder<int, MyPaths>{{ }}

public class Ut{{
    public void Test(){{
        new MyBuilder().With(p => p.Member.Value(3));
    }}
 }}
";

        var filePath = "testPath";

        var workspace = new RoslynWorkspaceBuilder()
            .AddDocument(filePath, sourceCode)
            .AddReference(typeof(int).Assembly.Location)
            .AddReference(typeof(ItemBuilder<>).Assembly.Location)
            .Build();

        var typeDescription = new TypeDescriptionBuilder()
            .WithDeclaredProperties(
                new PropertyDescriptionBuilder()
                    .WithReturnType(new TypeDescriptionBuilder().AsBaseType().Build())
                    .WithBaseType(true)
                    .WithName("Member")
                    .Build())
            .Build();

        var roslynContextQuery = new RoslynContextQuery(workspace);

        var memberConfigurationFinder = new RoslynMemberConfigurationFinder(new Discoverer());
        var descriptionFactory = Mock.Of<IRoslynDescriptionFactory>(factory =>
            factory.CreateDescription(It.IsAny<ITypeSymbol>()) == typeDescription);

        var sut = new RoslynConfigurationItemReader(
            new ConfigurationItemFactoryBuilder().Build(),
            roslynContextQuery,
            memberConfigurationFinder,
            descriptionFactory);

        var context = await roslynContextQuery.GetContextAsync(filePath)
            .GetSuccessUnsafeAsync();

        var builderInformation = new RoslynConfigFinder().FindConfigClass(
                RegexSpan.CreateWithStringMatch(sourceCode, "new MyBuilder()"),
                context)
            .GetValueUnsafe();

        var readConfigurationItems = await sut.ReadConfigurationItemsAsync(builderInformation, CancellationToken.None);

        readConfigurationItems.Should().HaveCount(1);
        var pair = readConfigurationItems.First();
        pair.Value.MemberConfigurations.Should().HaveCount(1);
        var memberConfig = pair.Value.MemberConfigurations["Member"];
        var valueMemberConfiguration = memberConfig.Should().BeAssignableTo<ValueMemberConfiguration>().Subject;
        valueMemberConfiguration.Value.Should().Be(new SimpleLiteralValue("3"));
        valueMemberConfiguration.Name.Should().Be("Member");
        valueMemberConfiguration.Source.Should().BeAssignableTo<FromCode>();
    }
}