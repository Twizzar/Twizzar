using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Common.Messaging.Events;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Infrastructure.VisualStudio.Services;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Services;

[TestFixture]
public class VersionCheckerTests
{
    private Func<TwizzarAnalyzerAddedEvent, Task> _invokeEventAsync;
    private IItemContext<VersionChecker, TwizzarVersionCheckerPaths> _context;

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<VersionChecker>()
            .ShouldThrowArgumentNullException();
    }

    [SetUp]
    public void Setup()
    {
        this._invokeEventAsync = null;

        var eventHub = new Mock<IUiEventHub>();
        eventHub.Setup(hub => hub.Subscribe(It.IsAny<object>(), It.IsAny<Func<TwizzarAnalyzerAddedEvent, Task>>()))
            .Callback<object, Func<TwizzarAnalyzerAddedEvent, Task>>((h, a) =>
            {
                this._invokeEventAsync = a;
            });

        var sut = new TwizzarVersionCheckerBuilder("1.0.0")
            .With(p => p.Ctor.eventHub.Value(eventHub.Object))
            .Build(out this._context);
        sut.Initialize();
    }

    [TestCase(1, 1)]
    [TestCase(2, 0)]
    public async Task Notification_is_send_when_major_or_minor_is_changed(int major, int minor)
    { 
        await this._invokeEventAsync(new TwizzarAnalyzerAddedEvent(new Version(major, minor , 0)));

        this._context.Verify(p => p.Ctor.notificationService.SendToInfoBarAsync)
            .Called(1);

        this._context.Verify(p => p.Ctor.notificationService.SendToOutputAsync)
            .Called(1);
    }

    [TestCase(0)]
    [TestCase(1)]
    public async Task Notification_is_not_send_when_patch_is_changed(int patch)
    {
        await this._invokeEventAsync(new TwizzarAnalyzerAddedEvent(new Version(1, 0, patch)));

        this._context.Verify(p => p.Ctor.notificationService.SendToInfoBarAsync)
            .Called(0);

        this._context.Verify(p => p.Ctor.notificationService.SendToOutputAsync)
            .Called(0);
    }

    public class TwizzarVersionCheckerBuilder : ItemBuilder<VersionChecker, TwizzarVersionCheckerPaths>
    {
        /// <inheritdoc />
        public TwizzarVersionCheckerBuilder(string version)
        {
            this.With(p => p.Ctor.addinVersionQuery.GetVsAddinVersion.Value(version));
        }
    }
}