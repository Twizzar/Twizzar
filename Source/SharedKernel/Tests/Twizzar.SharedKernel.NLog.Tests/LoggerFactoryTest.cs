using System;

using FluentAssertions;

using NUnit.Framework;

using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

namespace Twizzar.SharedKernel.NLog.Tests
{
    [TestFixture]
    public class LoggerFactoryTest
    {
        [Test]
        [Ignore("This test only works when called first.")]
        public void use_factory_without_configuration_throws_exception()
        {
            // arrange
            var expectedErrorMessage = "Logger is not yet configured, please first call method SetConfig.";

            // act
            Action act = () => LoggerFactory.GetLogger(typeof(TestLogger));

            // assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void use_factory_with_configuration_null_type_throws_exception()
        {
            // arrange
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder());

            // act
            Action act = () => LoggerFactory.GetLogger((string)null);

            // assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void use_factory_with_configuration_and_type()
        {
            // arrange
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder());

            // act
            var sut = LoggerFactory.GetLogger(typeof(TestLogger));

            // assert
            sut.Should().NotBeNull().And.BeAssignableTo<ILogger>();
            sut.Should().NotBeNull().And.BeOfType<NLogLoggerWrapper>();
        }

        [Test]
        public void use_factory_with_configuration_file()
        {
            // act
            Action act = () => LoggerFactory.SetConfig("NLog.config");

            // assert
            act.Should().NotThrow();
        }

        [Test]
        public void use_factory_with_wrong_configuration_file_throws_exception()
        {
            // arrange
            var errorMessage = "Invalid logger configuration";

            // act
            Action act = () => LoggerFactory.SetConfig("NLog-invalid.config");

            // assert
            act.Should().Throw<ArgumentException>().WithMessage(errorMessage);
        }

        [Test]
        public void use_factory_with_wrong_configuration_file_path_throws_exception()
        {
            // arrange
            var filePath = "non-available-file.config";
            var errorMessage = "Config file not found:" + filePath;

            // act
            Action act = () => LoggerFactory.SetConfig(filePath);

            // assert
            act.Should().Throw<ArgumentException>().WithMessage(errorMessage);
        }

        class TestLogger
        {
        }
    }
}
