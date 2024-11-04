using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating.Tests;

[TestFixture, TestOf(typeof(TemplateFileQuery))]
public class TemplateFileQueryTests
{
    private async Task<ITemplateFile> ArrangeAndAct()
    {
        // arrange
        var defaultReader = await new TemplateFileService().GetDefaultFileReaderAsync();

        var fileService = new ItemBuilder<ITemplateFileService>()
            .With(p => p.GetFileReaderAsync.Value(
                Task.FromResult(Maybe.Some(defaultReader))))
            .Build();

        var baseParser = new BaseConfigParser();
        
        var sut = new TemplateFileQuery(new TemplateSnippetFactory(), fileService, baseParser);

        // act
        return await sut.GetAsync("@\"$solution$\\generator-templates.twizzar.config\"", CancellationToken.None)
            .Map(result => result.GetSuccessUnsafe());
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<TemplateFileQuery>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task FileSnipped_from_default_loads_all_tags()
    {
        var file = await this.ArrangeAndAct();

        // assert
        file.Snippets.Should().HaveCount(23);
    }

    [Test]
    public async Task FileSnipped_file_is_correctly_parsed()
    {
        var file = await this.ArrangeAndAct();

        // assert
        var snippet = file.GetSingleSnipped(SnippetType.File);
        snippet.Type.Should().Be(SnippetType.File);
        snippet.TagUsage.Should().Be("<test-file>");
        snippet.Content.Should().Contain($@"<test-usings>

namespace $testNamespace$
{{
    <test-class>
}}");
    }

    [Test]
    public async Task FileSnipped_usings_is_correctly_parsed()
    {
        var file = await this.ArrangeAndAct();

        // assert
        var snippet = file.Snippets.Values.SelectMany(s => s).Single(s => s.TagUsage == "<test-usings>");
        snippet.Type.Should().Be(SnippetType.Default);
        snippet.TagUsage.Should().Be("<test-usings>");
        snippet.Content.Should().Contain($@"using NUnit.Framework;
using Twizzar.Fixture;
<argument-usings>");
    }
}