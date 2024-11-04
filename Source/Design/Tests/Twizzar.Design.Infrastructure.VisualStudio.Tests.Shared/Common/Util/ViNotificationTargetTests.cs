using System;

using Moq;

using NLog.Targets;

using NUnit.Framework;

using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Common.Util;
using Twizzar.Fixture;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using Twizzar.TestCommon;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Common.Util;

[TestFixture]
public class ViNotificationTargetTests
{
    private Mock<IViNotificationService> _notificationServiceMock;
    private ViNotificationTarget _sut;
    private ILogger _logger;

    [SetUp]
    public void SetUp()
    {
        var notificationService = Build.New<IViNotificationService>();
        this._sut = new ViNotificationTarget(notificationService);

        this._notificationServiceMock = Mock.Get(notificationService);

        this._logger = GetLogger(this._sut);
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ViNotificationTarget>()
            .ShouldThrowArgumentNullException();
    }

    private static ILogger GetLogger(Target target)
    {
        LoggerFactory.SetConfig(new LoggerConfigurationBuilder()
            .AddTarget(target));

        return LoggerFactory.GetLogger(typeof(ViNotificationTargetTests));
    }

    [Test]
    public void For_info_only_ShowOutput_is_called()
    {
        var message = Build.New<string>();
        this._logger.Log(LogLevel.Info, message);

        this._notificationServiceMock.Verify(
            service => service.SendToOutputAsync(It.Is<string>(s => s.Contains(message))),
            Times.Once);

        this._notificationServiceMock.Verify(
            service => service.SendToInfoBarAsync(It.IsAny<string>()),
            Times.Never());
    }

    [Test]
    public void For_fatal_ShowOutput_and_ShowInfoBar_is_called()
    {
        var message = Build.New<string>();
        this._logger.Log(LogLevel.Fatal, message);

        this._notificationServiceMock.Verify(
            service => service.SendToOutputAsync(It.Is<string>(s => s.Contains(message))),
            Times.Once);

        this._notificationServiceMock.Verify(
            service => service.SendToInfoBarAsync(It.Is<string>(s => s.Contains(message))),
            Times.Once());
    }

    [Test]
    public void Exceptions_get_captured_and_logged()
    {
        this._notificationServiceMock
            .Setup(service => service.SendToOutputAsync(It.IsAny<string>()))
            .Throws<Exception>();

        var mockLogger = new Mock<ILogger>();
        var message = Build.New<string>();

        this._sut.Logger = mockLogger.Object;
        this._logger.Log(LogLevel.Info, message);

        mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<Exception>()), Times.Once());
    }
}