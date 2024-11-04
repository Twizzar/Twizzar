using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.IntraTextAdornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.FixtureItem.Adornment.IntraTextAdornment;

[TestFixture]
public class VsTextViewTaggerTests
{
    private IVsProjectManager _vsProjectManager;
    private IVsEventCache _vsEventCache;

    [SetUp]
    public void SetUp()
    {
        this._vsProjectManager = new ItemBuilder<IVsProjectManager>()
            .With(p => p.FindProjectName.Value(Maybe.Some("TestProject")))
            .Build();

        this._vsEventCache = new ItemBuilder<IVsEventCache>()
            .With(p => p.AllReferencesAreLoaded.Value(true))
            .Build();
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<VsTextViewTagger>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void GetTags_calls_ViDocumentTagger_when_setup_correctly()
    {
        // arrange
        var viDocumentTagger = new ItemBuilder<IViDocumentTagger>()
            .With(p => p.GetTags.Value(
                Enumerable.Empty<ITagSpan<IntraTextAdornmentTag>>()))
            .With(p => p.IsDisposes.Value(false))
            .Build();

        var factory = CreateViDocumentTaggerFactory(viDocumentTagger);

        var snapshotSpans = new ItemBuilder<SnapshotSpan>()
            .With(p => p.Ctor.snapshot.Length.Value(10))
            .With(p => p.Ctor.start.Value(0))
            .With(p => p.Ctor.length.Value(5))
            .BuildMany(1);

        var spans = new NormalizedSnapshotSpanCollection(snapshotSpans);
            
        var sut = this.BaseSutConfigWithFactory(factory).Build();

        // act
        sut.GetTags(spans);

        // assert
        Mock.Get(viDocumentTagger)
            .Verify(tagger => tagger.GetTags(spans), Times.Once);
    }

    private ItemBuilder<VsTextViewTagger> BaseSutConfigWithFactory(IViDocumentTaggerFactory factory) =>
        this.BaseSutConfig()
            .With(p => p.Ctor.documentTaggerFactory.Value(factory));

    private ItemBuilder<VsTextViewTagger> BaseSutConfig() =>
        new ItemBuilder<VsTextViewTagger>()
            .With(p => p.Ctor.vsProjectManager.Value(this._vsProjectManager))
            .With(p => p.Ctor.eventCache.Value(this._vsEventCache));

    private static IViDocumentTaggerFactory CreateViDocumentTaggerFactory(IViDocumentTagger viDocumentTagger) =>
        new ItemBuilder<IViDocumentTaggerFactory>()
            .With(p => p.Create.Value(viDocumentTagger))
            .Build();

    [Test]
    public void Dispose_unsubscribes_events()
    {
        // arrange
        var eventHubMock = new Mock<IUiEventHub>();

        var snapshot = new ItemBuilder<ITextSnapshot>()
            .With(p => p.Length.Value(20))
            .With(p => p.Version.VersionNumber.Value(1))
            .Build();
            
        
            //Build.New<ITextSnapshot>(new EmptyBuilders.EmptyITextSnapshotConfig()
            //.Property(EmptyBuilders.EmptyITextSnapshotConfig.Length)
            //.Value(20)
            //.Property(EmptyBuilders.EmptyITextSnapshotConfig.Version)
            //.Value(
            //    Build.New<ITextVersion>(new EmptyBuilders.EmptyITextVersionConfig()
            //        .Property(EmptyBuilders.EmptyITextVersionConfig.VersionNumber)
            //        .Value(1))));

            var expectedDocumentSpan = new SnapshotSpan(
                new ItemBuilder<SnapshotPoint>()
                    .With(p => p.Ctor.position.Value(0))
                    .With(p => p.Ctor.snapshot.Value(snapshot))
                    .Build(),
                new ItemBuilder<SnapshotPoint>()
                    .With(p => p.Ctor.position.Value(20))
                    .With(p => p.Ctor.snapshot.Value(snapshot))
                    .Build());

            var viDocumentTagger = new ItemBuilder<IViDocumentTagger>()
                .With(p => p.GetDocumentSpan.Value(expectedDocumentSpan))
                .Build();

        var factory = CreateViDocumentTaggerFactory(viDocumentTagger);

        var sut = this.BaseSutConfigWithFactory(factory)
                .With(p => p.Ctor.eventHub.Value(eventHubMock.Object))
                .Build();

        // act
        sut.Dispose();

        // assert
        eventHubMock.Verify(hub => hub.Unsubscribe(sut, It.IsAny<Action<AdornmentSizeChangedEvent>>()), Times.Once);

        eventHubMock.Verify(
            hub => hub.Unsubscribe(sut, It.IsAny<Action<ProjectReferencesLoadedEvent>>()),
            Times.Once);
    }

    [Test]
    public void After_ProjectReferenceLoadedEvent_TagsChanged_is_called()
    {
        // arrange
        var eventHub = new UiEventHubStub();

        var factory = CreateViDocumentTaggerFactory(Build.New<IViDocumentTagger>());

        this._vsEventCache = new ItemBuilder<IVsEventCache>()
            .With(p => p.AllReferencesAreLoaded.Value(false))
            .Build();

        var sut = this.BaseSutConfigWithFactory(factory)
                .With(p => p.Ctor.eventHub.Value(eventHub))
                .Build();

        // act
        int count = 0;
        sut.TagsChanged += (sender, args) => count++;

        eventHub.Publish(new ProjectReferencesLoadedEvent(Mock.Of<IViProject>( p => p.Name == "TestProject")));

        // assert
        count.Should().Be(1);
    }

    [Test]
    public void After_AdornmentSizeChangedEvent_TagsChanged_is_called()
    {
        // arrange
        var eventHub = new UiEventHubStub();

        var documentTagger = new ItemBuilder<IViDocumentTagger>()
            .With(p => p.GetAffectedSpan.Value(Maybe.Some(new SnapshotSpan())))
            .Build();
            
        var factory = CreateViDocumentTaggerFactory(documentTagger);

        var sut = this.BaseSutConfigWithFactory(factory)
                .With(p => p.Ctor.eventHub.Value(eventHub))
                .Build();

        // act
        int count = 0;
        sut.TagsChanged += (sender, args) => count++;

        eventHub.Publish(Build.New<AdornmentSizeChangedEvent>());

        // assert
        count.Should().Be(1);
    }

    [Test]
    public void Update_creates_a_new_DocumentTagger()
    {
        // arrange
        var factory = CreateViDocumentTaggerFactory(Build.New<IViDocumentTagger>());

        var sut = this.BaseSutConfigWithFactory(factory).Build();
        var documentFilePath = Build.New<string>();
            
        // act
        sut.Update(documentFilePath);

        // assert
        Mock.Get(factory).Verify(factory => factory.Create(
            It.IsAny<IWpfTextView>(),
            It.IsAny<IPeekBroker>(),
            documentFilePath,
            "TestProject"), Times.Once);
    }

    [Test]
    public void Update_throws_when_null_is_provided()
    {
        // arrange
        var sut = Build.New<VsTextViewTagger>();

        // act 
        sut.Dispose();
        var a = () => sut.Update(null);

        // assert
        a.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Update_throws_when_empty_string_is_provided()
    {
        // arrange
        var sut = Build.New<VsTextViewTagger>();

        // act 
        sut.Dispose();
        Action a = () => sut.Update(string.Empty);

        // assert
        a.Should().Throw<ArgumentException>();
    }
}

public class UiEventHubStub : IUiEventHub
{
    #region Implementation of IUiEventHub

    private readonly Dictionary<Type, object> _subscriptions = new Dictionary<Type, object>();

    /// <inheritdoc />
    public void Subscribe<T>(object subscriber, Action<T> handler)
        where T : IUiEvent
    {
        this._subscriptions.Add(typeof(T), handler);
    }

    /// <inheritdoc />
    public void Subscribe<T>(object subscriber, Func<T, Task> handler) where T : IUiEvent
    {
        this._subscriptions.Add(typeof(T), handler);
    }

    /// <inheritdoc />
    public void Unsubscribe<T>(object subscriber, Action<T> handler)
        where T : IUiEvent
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Publish<T>(T uiEvent)
        where T : IUiEvent
    {
        var handler = (Action<T>)this._subscriptions[typeof(T)];
        handler.Invoke(uiEvent);
    }

    #endregion
}