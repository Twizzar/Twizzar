using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Enums;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Configs;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.FixtureItem.Adornment;

[TestFixture]
public class DocumentAdornmentControllerTests
{
    private SnapshotSpan _snapshotSpan;
    private IViAdornment _dummyAdornment;
    private IWpfTextView _wpfTextView;

    [SetUp]
    public void SetUp()
    {
        this._wpfTextView = new ItemBuilder<IWpfTextView>()
            .With(p => p.Properties.Value(Build.New<PropertyCollection>()))
            .Build();

        this._snapshotSpan = new EmptyConfigs.DefaultSnapShotConfigBuilder().Build();
        this._dummyAdornment = Build.New<IViAdornment>();
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // asserts
        Verify.Ctor<DocumentAdornmentController>()
            .SetupParameter("view", this._wpfTextView)
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task AdornmentSession_is_disposed_on_disposed()
    {
        var adornmentSession = Build.New<IAdornmentSession>();

        var sut = this.BaseSutBuilderWithFactory(adornmentSession).Build();

        await sut.OpenAdornmentAsync(
            this._dummyAdornment,
            this._snapshotSpan);
        sut.Dispose();

        Mock.Get(adornmentSession)
            .Verify(session => session.Dispose(), Times.Once);
    }

    [Test]
    public void FixtureItemPeekResultContent_is_disposed_on_Disposed()
    {
        var fixtureItemPeekResultContent = Build.New<IFixtureItemPeekResultContent>();

        var sut = this.BaseSutBuilder()
            .With(p => p.Ctor.fixtureItemPeekResultContent.Value(fixtureItemPeekResultContent))
            .Build();

        sut.Dispose();

        Mock.Get(fixtureItemPeekResultContent)
            .Verify(content => content.Dispose(), Times.Once);
    }

    [Test]
    public async Task AdornmentSession_is_closed_on_CloseAdornmentAsync()
    {
        var adornmentSession = Build.New<IAdornmentSession>();

        var sut = this.BaseSutBuilderWithFactory(adornmentSession).Build();

        await sut.OpenAdornmentAsync(
            this._dummyAdornment,
            this._snapshotSpan);

        await sut.CloseAdornmentAsync(this._dummyAdornment);

        Mock.Get(adornmentSession)
            .Verify(session => session.CloseAsync(), Times.Once);
    }

    [Test]
    public async Task When_a_new_adornment_is_opened_close_the_old_adornmentSession()
    {
        var adornmentSession = Build.New<IAdornmentSession>();

        var sut = this.BaseSutBuilderWithFactory(adornmentSession).Build();

        await sut.OpenAdornmentAsync(
            this._dummyAdornment,
            this._snapshotSpan);

        await sut.OpenAdornmentAsync(
            this._dummyAdornment,
            this._snapshotSpan);

        Mock.Get(adornmentSession)
            .Verify(session => session.CloseAsync(), Times.Once);
    }

    [Test]
    public async Task OpenAdornment_on_exception_log_to_logger()
    {
        var exp = new Exception();
        IAdornmentSession ThrowMethod() =>
            throw exp;

        var sut = this.BaseSutBuilder()
            .With(p => p.Ctor.adornmentSessionFactory.CreateAndStartAsync.Value(Task.Run(ThrowMethod)))
            .Build();

        sut.Logger = Build.New<ILogger>();

        await sut.OpenAdornmentAsync(this._dummyAdornment, this._snapshotSpan);

        Mock.Get(sut.Logger)
            .Verify(logger => logger.Log(LogLevel.Error, exp), Times.Once);
    }

    [Test]
    public async Task OpenClose_on_exception_log_to_logger()
    {
        var exp = new Exception();
        IAdornmentSession ThrowMethod() =>
            throw exp;

        var adornmentSession = new ItemBuilder<IAdornmentSession>()
            .With(p => p.CloseAsync.Value(Task.Run(ThrowMethod)))
            .Build();

        var sut = this.BaseSutBuilderWithFactory(adornmentSession).Build();

        sut.Logger = Build.New<ILogger>();

        await sut.OpenAdornmentAsync(this._dummyAdornment, this._snapshotSpan);
        await sut.CloseAdornmentAsync(this._dummyAdornment);

        Mock.Get(sut.Logger)
            .Verify(logger => logger.Log(LogLevel.Error, exp), Times.Once);
    }

    [Test]
    public async Task UpdateAsync()
    {
        var information = Build.New<IAdornmentInformation>();
        var fixtureItemPeekResultContent = Build.New<IFixtureItemPeekResultContent>();

        var sut = this.BaseSutBuilder()
            .With(p => p.Ctor.fixtureItemPeekResultContent.Value(fixtureItemPeekResultContent))
            .Build();

        await sut.UpdateInformationAsync(information);

        Mock.Get(fixtureItemPeekResultContent)
            .Verify(content => content.UpdateAsync(information), Times.Once);
    }

    [Test]
    public void FocusFixturePanel_calls_FocusFixturePanel_on_IFixtureItemPeekResultContent()
    { 
        var thread = new Thread(() =>
        {
            var peekResultContent = new Mock<IFixtureItemPeekResultContent>();

            peekResultContent.Setup(content => content.ScrollViewer)
                .Returns(Mock.Of<UIElement>);
            //this._fixture.SetInstance(peekResultContent.Object);


            var sut = this.BaseSutBuilder()
                .With(p => p.Ctor.fixtureItemPeekResultContent.Value(peekResultContent.Object))
                .Build();

            sut.FocusFixturePanel(ViEnterFocusPosition.Last);
            peekResultContent.Verify(content => content.MoveFocus(ViEnterFocusPosition.Last), Times.Once);
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
    }

    private ItemBuilder<DocumentAdornmentController> BaseSutBuilderWithFactory(IAdornmentSession adornmentSession) =>
        this.BaseSutBuilder()
            .With(p => p.Ctor.adornmentSessionFactory.CreateAndStartAsync.Value(Task.FromResult(adornmentSession)));

    private ItemBuilder<DocumentAdornmentController> BaseSutBuilder() =>
        new ItemBuilder<DocumentAdornmentController>()
            .With(p => p.Ctor.view.Value(this._wpfTextView));
}