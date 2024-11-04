using System.Collections.Generic;
using System.Collections.Immutable;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Mapping;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Mapping;

[TestFixture]
public class MappingReplacementServiceTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<MappingReplacementService>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Replace_from_config_with_not_variables_returns_equivalent_config()
    {
        // arrange 
        var sut = new ItemBuilder<MappingReplacementService>()
            .Build();

        var config = new MappingConfigBuilder(ImmutableArray<MappingEntry>.Empty)
            .Build();

        var info = new ItemBuilder<CreationInfo>()
            .Build();

        //act
        var resultConfig = sut.ReplacePlaceholders(config, info);

        // assert
        resultConfig.Should().BeEquivalentTo(config);
    }

    [TestCase("$projectUnderTest$")]
    [TestCase("$fileUnderTest$")]
    [TestCase("$typeUnderTest$")]
    [TestCase("$namespaceUnderTest$")]
    [TestCase("$memberUnderTest$")]
    public void Variable_get_replaced_with_the_correct_replacement(string variable)
    {
        // arrange
        var sut = new ItemBuilder<MappingReplacementService>()
            .Build();

        var prefix = TestHelper.RandomString();

        var entries = ImmutableArray<MappingEntry>.Empty
            .Add(new MappingEntry(Maybe.None(), $"{prefix}{variable}"));

        var config = new MappingConfigBuilder(entries)
            .Build();

        var info = new ItemBuilder<CreationInfo>()
            .Build();

        var variableToValue = new Dictionary<string, string>
        {
            { "$projectUnderTest$", info.Project},
            { "$fileUnderTest$", info.File},
            { "$typeUnderTest$", info.Type},
            { "$namespaceUnderTest$", info.Namespace},
            { "$memberUnderTest$", info.Member},
        };

        var expectedEntries = ImmutableArray<MappingEntry>.Empty
            .Add(new MappingEntry(Maybe.None(), $"{prefix}{variableToValue[variable]}"));
        var expectedConfig = new MappingConfigBuilder(expectedEntries)
            .Build();

        //act
        var resultConfig = sut.ReplacePlaceholders(config, info);

        // assert
        resultConfig.Should().Be(expectedConfig);
    }


    [Test]
    public void For_project_and_file_only_the_name_without_the_directory_will_be_inserted()
    {
        // arrange
        var sut = new ItemBuilder<MappingReplacementService>()
            .Build();

        const string projectName = "TestProject";
        const string fileName = "TestFile";

        var entries = ImmutableArray<MappingEntry>.Empty
            .Add(new MappingEntry(Maybe.None(), $"$projectUnderTest$$fileUnderTest$"));

        var config = new MappingConfigBuilder(entries)
            .Build();

        var info = new ItemBuilder<CreationInfo>()
            .With(p => p.Ctor.Project.Value($"{TestHelper.RandomString()}/{projectName}"))
            .With(p => p.Ctor.File.Value($"{TestHelper.RandomString()}/{fileName}"))
            .Build();

        var expectedEntries = ImmutableArray<MappingEntry>.Empty
            .Add(new MappingEntry(Maybe.None(), $"{projectName}{fileName}"));
        var expectedConfig = new MappingConfigBuilder(expectedEntries)
            .Build();

        //act
        var resultConfig = sut.ReplacePlaceholders(config, info);

        // assert
        resultConfig.Should().Be(expectedConfig);
    }
}