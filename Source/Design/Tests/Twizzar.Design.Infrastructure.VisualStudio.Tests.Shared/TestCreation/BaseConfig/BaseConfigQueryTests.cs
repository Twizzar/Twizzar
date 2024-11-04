using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.BaseConfig;

[TestFixture]
public class BaseConfigQueryTests
{
    #region members

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<BaseConfigQuery>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task When_config_file_not_exists_throw_ConfigFilesNotExistsException()
    {
        // arrange

        // create a base config query where the config file does not exist
        var sut = new BaseConfigQueryBuilder()
            .With(p => p.Ctor.configQuery.GetConfigAsync.Value(
                Result.FailureAsync<TestCreationConfig, Failure>(new ConfigFileNotExistsFailure("TestFile"))))
            .Build();

        var info = new ItemBuilder<CreationInfo>()
            .Build();

        // act
        var func = () => sut.GetOrCreateConfigsAsync(info, CancellationToken.None);

        // assert
        var exceptionAssertions = await func.Should().ThrowAsync<ConfigFilesNotExistsException>();
        exceptionAssertions.And.Files.Should().Contain("TestFile");
    }

    [Test]
    public async Task When_template_file_not_exists_throw_ConfigFilesNotExistsException()
    {
        // arrange

        // create a base config query where the template file does not exist
        var sut = new BaseConfigQueryBuilder()
            .With(p => p.Ctor.templateFileQuery.GetAsync.Value(
                Result.FailureAsync<ITemplateFile, Failure>(
                    new ConfigFileNotExistsFailure("TestFile"))))
            .Build();

        var info = new ItemBuilder<CreationInfo>()
            .Build();

        // act
        var func = () => sut.GetOrCreateConfigsAsync(info, CancellationToken.None);

        // assert
        var exceptionAssertions = await func.Should().ThrowAsync<ConfigFilesNotExistsException>();
        exceptionAssertions.And.Files.Should().Contain("TestFile");
    }

    [Test]
    public void When_both_not_exists_throw_ConfigFilesNotExistsException()
    {
        // arrange

        // create a base config query where the template file does not exist
        var sut = new ItemBuilder<BaseConfigQuery>()
            .With(p => p.Ctor.templateFileQuery.GetAsync.Value(
                Result.FailureAsync<ITemplateFile, Failure>(
                    new ConfigFileNotExistsFailure("TestFile1"))))

            // add a dummy config query where the config file is loaded successfully
            .With(p => p.Ctor.configQuery.GetConfigAsync.Value(
                Result.FailureAsync<TestCreationConfig, Failure>(
                    new ConfigFileNotExistsFailure("TestFile2"))))
            .Build();

        var info = new ItemBuilder<CreationInfo>()
            .Build();

        // act
        var func = () => sut.GetOrCreateConfigsAsync(info, CancellationToken.None);

        // assert
        var exceptionAssertions = func.Should().Throw<ConfigFilesNotExistsException>();
        exceptionAssertions.And.Files.Should().Contain("TestFile1");
        exceptionAssertions.And.Files.Should().Contain("TestFile2");
    }

    [Test]
    public async Task When_template_file_and_configuration_are_successfully_found_and_parsed_they_will_be_returned()
    {
        // arrange
        var sut = new BaseConfigQueryBuilder()
            .Build();

        // act
        var result = await sut.GetOrCreateConfigsAsync(new ItemBuilder<CreationInfo>().Build(), CancellationToken.None);

        // assert
        result.Config.Should().NotBeNull();
        result.TemplateFile.Should().NotBeNull();
    }

    [Test]
    public void When_the_templateFileQuery_fails_throw_InternalException()
    {
        // arrange
        var sut = new BaseConfigQueryBuilder()
            .With(p => p.Ctor.templateFileQuery.GetAsync.Value(
                Result.FailureAsync<ITemplateFile, Failure>(
                    new Failure("Test"))))
            .Build();

        // act
        var func = () => sut.GetOrCreateConfigsAsync(new ItemBuilder<CreationInfo>().Build(), CancellationToken.None);

        // assert
        func.Should().Throw<InternalException>();
    }

    [Test]
    public void When_the_configQuery_fails_throw_InternalException()
    {
        // arrange
        var sut = new BaseConfigQueryBuilder()
            .With(p => p.Ctor.configQuery.GetConfigAsync.Value(
                Result.FailureAsync<TestCreationConfig, Failure>(
                    new Failure("Test"))))
            .Build();

        // act
        var func = () => sut.GetOrCreateConfigsAsync(new ItemBuilder<CreationInfo>().Build(), CancellationToken.None);

        // assert
        func.Should().Throw<InternalException>();
    }

    #endregion

    #region Nested type: BaseConfigQueryBuilder

    private class BaseConfigQueryBuilder : ItemBuilder<BaseConfigQuery, BaseConfigQueryCustomPaths>
    {
        #region ctors

        public BaseConfigQueryBuilder()
        {
            this.With(p => p.Ctor.templateFileQuery.GetAsync.Value(
                Result.SuccessAsync<ITemplateFile, Failure>(
                    new ItemBuilder<ITemplateFile>()

                        // setup the WithBackupFile method to return a stubbed template file
                        .With(p => p.WithBackupFile.Stub<ITemplateFile>())
                        .Build())));

            this.With(p => p.Ctor.configQuery.GetConfigAsync.Value(
                Result.SuccessAsync<TestCreationConfig, Failure>(
                    new ItemBuilder<TestCreationConfig>().Build())));
        }

        #endregion
    }

    #endregion
}