
using FluentAssertions;
using Moq;

using NUnit.Framework;
using Twizzar.Design.Infrastructure.Query;
using Twizzar.Design.Infrastructure.Services;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.Tests.Query;

[TestFixture]
public class SettingsFileServiceTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<SettingsFileService>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Initialize_set_EnableAnalitics_correctly()
    {
        // assert
        var fileService = new Mock<IFileService>();

        fileService
            .Setup(service => service.ReadAllLines(It.IsAny<string>()))
            .Returns(() => new[] { "EnableAnalitics: false" });

        var sut = new ItemBuilder<SettingsFileService>()
            .With(paths => paths.Ctor.fileService.Value(fileService.Object))
            .Build();

        // act
        sut.Initialize();

        // assert
        sut.GetAnalyticsEnabled().Should().BeFalse();
    }

    [Test]
    public void SetAnalyticsEnabled_write_to_file()
    {
        // assert
        var fileService = new Mock<IFileService>();

        var sut = new ItemBuilder<SettingsFileService>()
            .With(paths => paths.Ctor.fileService.Value(fileService.Object))
            .Build();

        // act
        sut.SetAnalyticsEnabled(false);

        // assert
        fileService.Verify(service =>
            service.WriteAllLines(
                It.Is<string>(s => s.EndsWith(@"vi-sit\twizzar\twizzar.config")),
                It.Is<string[]>(a => a.Length == 1 && a[0].Contains("EnableAnalitics: false"))));

        sut.GetAnalyticsEnabled().Should().BeFalse();
    }
}