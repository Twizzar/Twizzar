using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;

using TestCreation.Services;

using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon;
using Twizzar.TestCommon.TypeDescription.Builders;

using TwizzarInternal.Fixture;

using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio2022.Tests.TestCreation.Services;

[TestFixture, TestOf(typeof(TemplateService))]
public class TemplateServiceTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<TemplateService>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task Method_return_type_is_added_to_additional_usings()
    {
        // arrange
        var sut = CreateSut();

        var member = new MethodDescriptionBuilder()
            .WithReturnType(new TypeDescriptionBuilder()
                .WithTypeFullName(TypeFullName.CreateFromType(typeof(int)))
                .Build())
            .Build();

        var source = new ItemBuilder<CreationContext>()
            .With(p => p.Ctor.SourceMember.Value(member))
            .Build();
        var destination = new ItemBuilder<CreationContext>().Build();

        // act
        var result = await sut.AddTemplate(source, destination, Build.New<ITemplateFile>());

        // assert
        var templateContext = result.TemplateContext.GetValueUnsafe();

        templateContext.AdditionalUsings.Should().Contain(typeof(int).Namespace);
    }

    [Test]
    public async Task Method_parameter_type_is_added_to_additional_usings()
    {
        // arrange
        var sut = CreateSut();

        var member = new MethodDescriptionBuilder()
            .WithReturnType(new TypeDescriptionBuilder()
                .WithTypeFullName(TypeFullName.CreateFromType(typeof(int)))
                .Build())
            .WithDeclaredParameter(new ParameterDescriptionBuilder()
                .WithReturnTypeDescription(new TypeDescriptionBuilder()
                    .WithTypeFullName(TypeFullName.CreateFromType(typeof(List<Task>)))
                    .WithGenericArgument(new GenericParameterType()
                    {
                        TypeFullName = TypeFullName.CreateFromType(typeof(Task))
                    })
                    .Build())
                .Build())
            .Build();

        var source = new ItemBuilder<CreationContext>()
            .With(p => p.Ctor.SourceMember.Value(member))
            .Build();
        var destination = new ItemBuilder<CreationContext>().Build();

        // act
        var result = await sut.AddTemplate(source, destination, Build.New<ITemplateFile>());

        // assert
        var templateContext = result.TemplateContext.GetValueUnsafe();

        templateContext.AdditionalUsings.Should()
            .Contain(typeof(Task).Namespace)
            .And
            .Contain(typeof(List<>).Namespace);
    }

    private static TemplateService CreateSut()
    {
        var file = new ItemBuilder<ITemplateFile>()
            .Build();

        var fileResult = Result.Success<ITemplateFile, Failure>(file);

        return new ItemBuilder<TemplateService>()
            .With(p => p.Ctor.templateFileQuery.GetAsync.Value(Task.FromResult(fileResult)))
            .Build();
    }
}