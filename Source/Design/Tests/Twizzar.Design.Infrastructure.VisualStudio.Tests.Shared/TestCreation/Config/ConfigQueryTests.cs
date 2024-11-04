using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Mapping;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using TwizzarInternal.Fixture.Member;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Config;

[TestFixture]
public class ConfigQueryTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ConfigQuery>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task When_config_file_not_exists_call_CreateDefaultFile()
    {
        // arrange
        var configQuery = new ConfigQueryBuilder()
            // setup the config file service to return None
            .With(p => p.Ctor.configFileService.GetFileReaderAsync.Value(Maybe.NoneAsync<TextReader>()))
            .Build(out var context);

        // act
        var config = await configQuery.GetConfigAsync(
            string.Empty,
            CancellationToken.None);

        // assert
        context.Verify(p => p.Ctor.configFileService.CreateDefaultFile)
            .Called(1);

        config.IsFailure.Should().BeTrue();
        config.GetFailureUnsafe().Should().BeAssignableTo<ConfigFileNotExistsFailure>();
    }

    [Test]
    public async Task When_file_exist_parse_config_syntax_correctly()
    {
        // arrange
        var configQuery = new ConfigQueryBuilder()
            .WithConfigSyntax(
                p => p.Ctor.ConfigVersion.Value(Version.Parse("1.0")),
                p => p.Ctor.Entries.Value(
                    ImmutableArray.Create<ConfigEntry>()
                        .Add(new ConfigEntry("testProjectPath", "testProjectPath"))
                        .Add(new ConfigEntry("testProject", "testProject"))
                        .Add(new ConfigEntry("testFile", "testFilePattern : testFileReplacement"))
                        .Add(new ConfigEntry("testFilePath", "testFilePath"))
                        .Add(new ConfigEntry("testNamespace", "testNamespace"))
                        .Add(new ConfigEntry("testClass", "testClass"))
                        .Add(new ConfigEntry("testClass", "testClass"))
                        .Add(new ConfigEntry("testMethod", "testMethod"))
                        .Add(new ConfigEntry("nugetPackages", "nugetPackages"))))
            .Build(out var context);

        // act
        var result = await configQuery.GetConfigAsync(string.Empty, CancellationToken.None);

        // assert
        context.Verify(p => p.Ctor.baseConfigParser.ParseBaseConfig)
            .Called(1);

        var config = TestHelper.AssertAndUnwrapSuccess(result);
        config.MappingConfig.ProjectMapping.Single().Replacement.Should().Be("testProject");
        config.MappingConfig.FileMapping.Single().Replacement.Should().Be("testFileReplacement");
        config.MappingConfig.FileMapping.Single().Pattern.GetValueUnsafe().Should().Be("testFilePattern");
    }

    private class ConfigQueryBuilder : ItemBuilder<ConfigQuery, ConfigQueryCustomPaths>
    {
        public ConfigQueryBuilder()
        {
            this.With(p => p.Ctor.configFileService.GetFileReaderAsync.Value(
                Maybe.SomeAsync<TextReader>(new StringReader(""))));
            this.With(p => p.Ctor.baseConfigParser.ParseBaseConfig.Value(ConfigSyntaxDefaultSuccess));
        }

        public ConfigQueryBuilder WithConfigSyntax(params Func<ConfigSyntaxPath, MemberConfig<ConfigSyntax>>[] memberConfigs)
        {
            var builder = new ItemBuilder<ConfigSyntax>();

            foreach (var memberConfig in memberConfigs)
            {
                builder.With(memberConfig);
            }

            this.With(p => p.Ctor.baseConfigParser.ParseBaseConfig.Value(Success(builder)));
            return this;
        }

        private static IResult<ConfigSyntax, ParseFailure> ConfigSyntaxDefaultSuccess =>
            Success(new ItemBuilder<ConfigSyntax>()
                .With(p => p.Ctor.ConfigVersion.Value(Version.Parse("1.0"))));

        private static Result<ConfigSyntax, ParseFailure> Success(ItemBuilderBase<ConfigSyntax> builder) =>
            builder.Build();
    }

}