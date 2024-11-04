using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Common.Util;

namespace Twizzar.Design.Ui.Messaging;

[TestFixture]
public class UiEventHubTests
{
    private UiEventHub _sut;
    private EventHandlerSpy _spy;

    [SetUp]
    public void SetUp()
    {
        this._sut = new();
        this._spy = new();
    }

    [Test]
    public void Subscribed_class_is_called_when_event_is_published()
    {
        // act
        this._sut.Subscribe<IUiEvent>(this._spy, this._spy.Handler);
        this._sut.Publish(new Mock<IUiEvent>().Object);

        // assert
        this._spy.CallCount.Should().Be(1);
    }

    [Test]
    public void When_unsubscribed_do_not_call_the_handler_method()
    {
        // act
        this._sut.Subscribe<IUiEvent>(this._spy, this._spy.Handler);
        this._sut.Publish(new Mock<IUiEvent>().Object);
        this._sut.Unsubscribe<IUiEvent>(this._spy, this._spy.Handler);
        this._sut.Publish(new Mock<IUiEvent>().Object);

        // assert
        this._spy.CallCount.Should().Be(1);
    }

    [Test]
    public async Task Subscribed_class_is_called_when_event_is_published_async()
    {
        this._sut.Subscribe<IUiEvent>(this._spy, this._spy.HandleAsync);
        this._sut.Publish(new Mock<IUiEvent>().Object);

        // assert
        await this._spy.HandleSemaphore.WaitAsync();
        this._spy.CallCount.Should().Be(1);
    }


    public class EventHandlerSpy
    {
        #region fields

        public SemaphoreSlim HandleSemaphore = new(1, 1);

        #endregion

        #region properties

        public int CallCount { get; private set; }

        public SynchronizationContext SynchronizationContext { get; private set; }

        #endregion

        #region members

        public void Handler(IUiEvent _)
        {
            this.CallCount++;
            this.SynchronizationContext = SynchronizationContext.Current;
        }

        public async Task HandleAsync(IUiEvent _)
        {
            await this.HandleSemaphore.WaitAsync();
            await Task.Yield();
            this.CallCount++;
            this.SynchronizationContext = SynchronizationContext.Current;

            this.HandleSemaphore.Release();
        }

        #endregion
    }
}