using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating.Tests;

[TestFixture]
public class TemplateVisitorTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<TemplateVisitor>()
            .ShouldThrowArgumentNullException();
    }


    [Test]
    public async Task Integration_test()
    {
        var fileQuery = new TemplateFileQuery(new TemplateSnippetFactory(), new TemplateFileService(), new BaseConfigParser());

        // act
        var file = await fileQuery.GetDefaultAsync();

        // assert
        var sut = new TemplateVisitor(new SnippetNodeFactory(new ItemBuilder<IShortTypesConverter>().Build()));

        var context = new ItemBuilder<ITemplateContext>()
            .With(p => p.File.Value(file))
            .With(p => p.SourceCreationContext
                .SourceMember.Value(Mock.Of<IMemberDescription>()))
            .With(p => p.AdditionalUsings.Value(
                new [] {"testUsing1", "testUsing2"}.ToImmutableHashSet()))
            .Build();

        var code = sut.Visit(file.GetSingleSnipped(SnippetType.File), context);
        Console.WriteLine(code);
    }
}