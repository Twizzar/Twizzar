using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

using Moq;

using NUnit.Framework;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.TestCommon;

using TwizzarInternal.Fixture;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.FixtureItem.Adornment;

[TestFixture]
public class AdornmentSessionTests
{
    private AdornmentSession _sut;
    private ITextView _textView;
    private SnapshotSpan _snapshotSpan;
    private IDocumentWriter _documentWriter;
    private IPeekBroker _peekBroker;
    private ItemBuilder<IFixtureItemPeekResultContent> _fixtureItemPeekResultContentBuilder;

    [SetUp]
    public void SetUp()
    {
        var adornmentInformation = Mock.Of<IAdornmentInformation>(
            information =>
                information.DocumentFilePath == TestHelper.RandomString("") &&
                information.ProjectName == TestHelper.RandomString("") &&
                information.FixtureItemId == TestHelper.RandomNamedFixtureItemId("", "", "") &&
                information.ObjectCreationSpan == Mock.Of<IViSpan>());

        var viAdornment = Mock.Of<IViAdornment>(adornment =>
            adornment.AdornmentInformation == adornmentInformation &&
            adornment.Id == AdornmentId.CreateNew(TestHelper.RandomString("")) &&
            adornment.StatusPanelViewModel == Mock.Of<IStatusPanelViewModel>());

        this._sut = new ItemBuilder<AdornmentSession>()
            .With(p => p.Ctor.viAdornment.Value(viAdornment))
            .With(p => p.Ctor.compilationTypeQueryFactory.Create.Stub<ICompilationTypeQuery>())
            .With(p => p.Ctor.compilationQuery.GetFromBufferAsync.Value(
                Maybe.SomeAsync(Task.FromResult<Compilation>(CSharpCompilation.Create("dummy")))))
            .Build();

        this._textView = new ItemBuilder<ITextView>()
            .Build();

        this._documentWriter = new ItemBuilder<IDocumentWriter>().Build();
        this._peekBroker = new ItemBuilder<IPeekBroker>().Build();

        this._snapshotSpan = new SnapshotSpanBuilder().Build();
        //this._snapshotSpan = new ItemBuilder<SnapshotSpan>(EmptyConfigs.DefaultSnapShotConfig).Build();
        this._fixtureItemPeekResultContentBuilder = new ItemBuilder<IFixtureItemPeekResultContent>();
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<AdornmentSession>()
            .ShouldThrowArgumentNullException();
    }

    [TestCase(nameof(AdornmentSession.CloseAsync))]
    [TestCase(nameof(AdornmentSession.Dispose))]
    public async Task CancellationToken_gets_canceled_on(string method)
    {
        // arrange
        Maybe<CancellationToken> cancellationToken = default;

        this._fixtureItemPeekResultContentBuilder
            .With(p => p.InitializeAsync.Callback((_, _, _, _, _, token) => cancellationToken = token))
            .With(p => p.InitializeAsync.Value(Task.CompletedTask));

        // act
        await this._sut.StartAsync(
            this._textView,
            this._snapshotSpan,
            this._fixtureItemPeekResultContentBuilder.Build(),
            this._documentWriter,
            this._peekBroker);

        if (method == nameof(AdornmentSession.CloseAsync))
        {
            await this._sut.CloseAsync();
        }
        else if (method == nameof(AdornmentSession.Dispose))
        {
            this._sut.Dispose();
        }
        else
        {
            Assert.Fail("method parameter is incorrect.");
        }

        // assert
        var token = cancellationToken.AsMaybeValue().Should().BeAssignableTo<SomeValue<CancellationToken>>().Subject;
        token.Value.IsCancellationRequested.Should().BeTrue();
    }

    [TestCase(nameof(AdornmentSession.CloseAsync))]
    [TestCase(nameof(AdornmentSession.Dispose))]
    public async Task Dispose_and_close_dismisses_peekSession(string method)
    {
        // arrange
        var session = new ItemBuilder<IPeekSession>()
            .Build();

        var sessionMock = Mock.Get(session);

        // when Dismiss is called set IsDismissed to true
        sessionMock.Setup(peekSession => peekSession.Dismiss())
            .Callback(() => sessionMock.Setup(peekSession => peekSession.IsDismissed).Returns(true));

        var peekBroker = new ItemBuilder<IPeekBroker>()
            .With(p => p.TriggerPeekSession.Value(session))
            .Build();

        // act
        await this._sut.StartAsync(this._textView, this._snapshotSpan, this._fixtureItemPeekResultContentBuilder.Build(), this._documentWriter, peekBroker);

        if (method == nameof(AdornmentSession.CloseAsync))
        {
            await this._sut.CloseAsync();
        }
        else if (method == nameof(AdornmentSession.Dispose))
        {
            this._sut.Dispose();
        }

        // assert
        Mock.Get(session)
            .Verify(peekSession => peekSession.Dismiss(), Times.Once);
    }

    [Test]
    public void Dispose_does_not_throw_when_start_is_not_called()
    {
        // act
        Action a = () => this._sut.Dispose();

        // assert
        a.Should().NotThrow();
    }
}

internal class SnapshotSpanBuilder : ItemBuilder<SnapshotSpan, DefaultSnapshotSpanPath>
{
    /// <inheritdoc />
    public SnapshotSpanBuilder()
    {
        this.With(p => p.Ctor.length.Value(10));
        this.With(p => p.Ctor.start.Value(0));
        this.With(p => p.Ctor.snapshot.Length.Value(20));
        this.With(p => p.Ctor.snapshot.Version.VersionNumber.Value(1));
        this.With(p => p.Ctor.snapshot.Version.CreateTrackingPoint.Stub<ITrackingPoint>());
        this.With(p => p.Ctor.snapshot.CreateTrackingPoint__Int32_PointTrackingMode.Stub<ITrackingPoint>());
    }
}