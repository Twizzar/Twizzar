using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestCreation.Services;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;
using Twizzar.Design.CoreInterfaces.TestCreation.Mapping;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Services;

[TestFixture, TestOf(typeof(MappingService))]
public class MappingServiceTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<MappingService>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task CreationInfo_content_will_be_replaced_by_patterMatcher_match_output()
    {
        var sut = new ItemBuilder<MappingService>()
            .With(p => p.Ctor.mappingReplacementService.ReplacePlaceholders.Value(
                new MappingConfigBuilder(ImmutableArray<MappingEntry>.Empty).Build()))
            .With(p => p.Ctor.patternMatcher.Match.Value(Result.Success<string, Failure>("test")))
            .Build();

        var info = new ItemBuilder<CreationInfo>()
            .Build();

        var config = new ItemBuilder<TestCreationConfig>()
            .Build();

        var newInfo = await sut.MapAsync(info, config);

        newInfo.File.Should().EndWith("test");
        newInfo.Member.Should().Be("test");
        newInfo.Namespace.Should().Be("test");
        newInfo.Project.Should().EndWith("test");
        newInfo.Type.Should().Be("test");
    }

    [Test]
    public async Task When_path_is_a_directory_throw_Exception()
    {
        var sut = new ItemBuilder<MappingService>()
            .With(p => p.Ctor.mappingReplacementService.ReplacePlaceholders.Value(
                new MappingConfigBuilder(ImmutableArray<MappingEntry>.Empty).Build()))
            .With(p => p.Ctor.patternMatcher.Match.Value(Result.Success<string, Failure>("test")))
            .Build();

        var info = new ItemBuilder<CreationInfo>()
            .With(p => p.Ctor.Project.Value("Some/Directory/"))
            .Build();

        var config = new ItemBuilder<TestCreationConfig>()
            .Build();

        Func<Task> a = () => sut.MapAsync(info, config);
        await a.Should().ThrowAsync<InternalException>();
    }

    [Test]
    public async Task When_pattern_match_fails_throw_exception()
    {
        var sut = new ItemBuilder<MappingService>()
            .With(p => p.Ctor.mappingReplacementService.ReplacePlaceholders.Value(
                new MappingConfigBuilder(ImmutableArray<MappingEntry>.Empty).Build()))
            .With(p => p.Ctor.patternMatcher.Match.Value(Result.Failure<string, Failure>(new Failure(""))))
            .Build();

        var info = new ItemBuilder<CreationInfo>()
            .Build();

        var config = new ItemBuilder<TestCreationConfig>()
            .Build();

        Func<Task> a = () => sut.MapAsync(info, config);
        await a.Should().ThrowAsync<InternalException>();
    }

    [Test]
    public async Task The_file_mapping_is_relative_to_the_project_folder()
    {
        // arrange
        var sut = new ItemBuilder<MappingService>()
            .With(p => p.Ctor.mappingReplacementService.ReplacePlaceholders.Value((mappingConfig, creationInfo) =>
                mappingConfig))
            .With(p => p.Ctor.patternMatcher.Match.Value((s, entries) =>
                entries
                .FirstOrNone()
                .Map(entry => entry.Replacement)
                .SomeOrProvided(s)
                .ToSuccess<string, Failure>()))
            .Build();

        var info = new ItemBuilder<CreationInfo>()
            .With(p => p.Ctor.Project.Value(@"C:\\MySolution\MyProject\MyProject.csproj"))
            .With(p => p.Ctor.File.Value(@"C:\\MySolution\MyProject\MyFolder\MyFile.cs") )
            .Build();

        var config = new ItemBuilder<TestCreationConfig>()
            .With(p => p.MappingConfig.Value(
                    new MappingConfigBuilder(ImmutableArray<MappingEntry>.Empty)
                    .With(p => p.FileMapping.Value(
                        ImmutableArray<MappingEntry>.Empty.Add(new MappingEntry(Maybe.None(), "MyFileTest"))))
                    .With(p => p.FilePathMapping.Value(
                        ImmutableArray<MappingEntry>.Empty.Add(new MappingEntry(Maybe.None(), "MyFolderTest"))))
                    .Build()))
            .Build();

        // act
        var resultInfo = await sut.MapAsync(info, config);

        // assert
        resultInfo.File.Should().Be(@"C:\MySolution\MyProject\MyFolderTest\MyFileTest.cs");
    }
}