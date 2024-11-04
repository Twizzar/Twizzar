using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.Text.Editor;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.FixtureItem.Adornment;

[TestFixture]
public class ViAdornmentCacheCreatorTests
{
    private IDocumentAdornmentController _documentAdornmentControllerDummy;

    [SetUp]
    public void SetUp()
    {
        this._documentAdornmentControllerDummy = Build.New<IDocumentAdornmentController>();
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ViAdornmentCacheCreator>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Different_snapshot_same_AdornmentInformation_returns_same_Adornment()
    {
        // arrange
        var expected = new Mock<IViAdornment>().Object;

        var creator = CreateCreator(expected);

        var textView = new Mock<IWpfTextView>().Object;
        var adornmentInformation = new AdornmentInformationBuilder().WithVersion(new ViSpanVersion(1)).Build();

        var version2 = new TextVersionBuilder()
            .WithVersion(2)
            .Build();

        var version1 = new TextVersionBuilder()
            .WithVersion(1)
            .WithNext(version2)
            .Build();

        var snapshot1 = new TextSnapshotBuilder()
            .WithLength(1024)
            .WithVersion(version1);

        var snapshot2 = new TextSnapshotBuilder()
            .WithLength(1028)
            .WithVersion(version2);

        var textBufferBuilder = new TextBufferBuilder()
            .WithCurrentSnapshot(snapshot1);

        var snapshotHistory = new SnapshotHistoryBuilder()
            .AddVersion(new ViSpanVersion(1))
            .AddVersion(new ViSpanVersion(2))
            .Build();

        var sut = CreateSut(creator, snapshotHistory);

        // act
        var result = sut.GetOrCreate(new [] { adornmentInformation }, textView, this._documentAdornmentControllerDummy);
        textBufferBuilder.WithCurrentSnapshot(snapshot2);
        var result2 = sut.GetOrCreate(new[] { adornmentInformation }, textView, this._documentAdornmentControllerDummy);

        // assert
        result.First().Should().Be(expected);
        result2.First().Should().Be(expected);

        Mock.Get(creator)
            .Verify(
                adornmentCreator => adornmentCreator.Create(
                    It.IsAny<IAdornmentInformation>(),
                    textView,
                    It.IsAny<ISnapshotHistory>(),
                    this._documentAdornmentControllerDummy),
                Times.Once);
    }

    [Test]
    public void Same_snapshot_same_adornmentInformation_returns_same_Adornment()
    {
        // arrange
        var expected = new Mock<IViAdornment>().Object;
        var creator = CreateCreator(expected);
                
        var textView = new Mock<IWpfTextView>().Object;
        var adornmentInformation = new AdornmentInformationBuilder()
            .WithObjectCreationSpan(0, 10)
            .WithVersion(new ViSpanVersion(1))
            .Build();
        var adornmentInformation2 = new AdornmentInformationBuilder()
            .WithObjectCreationSpan(0, 10)
            .WithVersion(new ViSpanVersion(1))
            .Build();


        var snapshotHistory = new SnapshotHistoryBuilder()
            .AddVersion(new ViSpanVersion(1))
            .Build();

        var sut = CreateSut(creator, snapshotHistory);

        // act
        var result = sut.GetOrCreate(new [] { adornmentInformation } , textView, this._documentAdornmentControllerDummy);
        var result2 = sut.GetOrCreate(new [] { adornmentInformation2 }, textView, this._documentAdornmentControllerDummy);

        // assert
        result.First().Should().Be(expected);
        result.First().Should().Be(result2.First());

        Mock.Get(creator)
            .Verify(
                adornmentCreator => adornmentCreator.Create(
                    It.IsAny<IAdornmentInformation>(),
                    textView,
                    It.IsAny<ISnapshotHistory>(),
                    this._documentAdornmentControllerDummy),
                Times.Once);
    }

    [Test]
    public void Same_snapshot_intersecting_adornmentInformation_returns_same_Adornment()
    {
        // arrange
        var version = new ViSpanVersion(1);
        var expected = new Mock<IViAdornment>().Object;

        var creator = CreateCreator(expected);
            
        var textView = new Mock<IWpfTextView>().Object;
        var adornmentInformation = new AdornmentInformationBuilder()
            .WithObjectCreationSpan(0, 10)
            .WithVersion(version)
            .Build();

        var adornmentInformation2 = new AdornmentInformationBuilder()
            .WithObjectCreationSpan(8, 20)
            .WithVersion(version)
            .Build();

        var snapshotHistory = new SnapshotHistoryBuilder()
            .AddVersion(version)
            .Build();

        var sut = CreateSut(creator, snapshotHistory);

        // act
        var result = sut.GetOrCreate(new []{ adornmentInformation }, textView, this._documentAdornmentControllerDummy);
        var result2 = sut.GetOrCreate(new [] { adornmentInformation2 }, textView, this._documentAdornmentControllerDummy);

        // assert
        result.First().Should().Be(expected);
        result.First().Should().Be(result2.First());

        Mock.Get(creator)
            .Verify(
                adornmentCreator => adornmentCreator.Create(
                    It.IsAny<IAdornmentInformation>(),
                    textView,
                    It.IsAny<ISnapshotHistory>(),
                    this._documentAdornmentControllerDummy),
                Times.Once);
    }

    [Test]
    public void Same_snapshot_not_intersecting_adornmentInformation_returns_other_Adornment()
    {
        // arrange
        var version = new ViSpanVersion(1);
        var expected = new Mock<IViAdornment>().Object;
        var creator = CreateCreator(expected);
            
        var textView = new Mock<IWpfTextView>().Object;
        var adornmentInformation = new AdornmentInformationBuilder()
            .WithObjectCreationSpan(0, 10)
            .WithVersion(version)
            .Build();
        var adornmentInformation2 = new AdornmentInformationBuilder()
            .WithObjectCreationSpan(20, 30)
            .WithVersion(version)
            .Build();


        var snapshotHistory = new SnapshotHistoryBuilder()
            .AddVersion(version)
            .Build();

        var sut = CreateSut(creator, snapshotHistory);

        // act
        var result = sut.GetOrCreate(new [] { adornmentInformation } , textView, this._documentAdornmentControllerDummy);
        var result2 = sut.GetOrCreate(new[] { adornmentInformation2 }, textView, this._documentAdornmentControllerDummy);

        // assert
        result.First().Should().Be(expected);
        result.First().Should().Be(result2.First());

        Mock.Get(creator)
            .Verify(
                adornmentCreator => adornmentCreator.Create(
                    It.IsAny<IAdornmentInformation>(),
                    textView,
                    It.IsAny<ISnapshotHistory>(),
                    this._documentAdornmentControllerDummy),
                Times.Exactly(2));
    }

    [Test]
    public void Test_insertion_above()
    {
        // arrange
        var version1 = new ViSpanVersion(1);

        var adornment1 = new Mock<IViAdornment>().Object;

        var creator = CreateCreator(adornment1);
            
        var textView = new Mock<IWpfTextView>().Object;

        var adornmentInformation1 = new List<IAdornmentInformation>()
        {
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(0, 10)
                .WithVersion(version1)
                .Build(),
        };

        var adornmentInformation2 = new List<IAdornmentInformation>()
        {
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(0, 10)
                .WithVersion(version1)
                .Build(),
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(20, 30)
                .WithVersion(version1)
                .Build(),
        };


        var snapshotHistory = new SnapshotHistoryBuilder()
            .AddVersion(version1)
            .Build();

        var sut = CreateSut(creator, snapshotHistory);

        // act
        var result = sut.GetOrCreate(adornmentInformation1.ToArray(), textView, this._documentAdornmentControllerDummy);
        var result2 = sut.GetOrCreate(adornmentInformation2.ToArray() , textView, this._documentAdornmentControllerDummy);

        // assert
        result.Should().HaveCount(1);
        result2.Should().HaveCount(2);

        result2.First().Should().Be(adornment1);

        Mock.Get(creator)
            .Verify(
                adornmentCreator => adornmentCreator.Create(
                    It.IsAny<IAdornmentInformation>(),
                    textView,
                    It.IsAny<ISnapshotHistory>(),
                    this._documentAdornmentControllerDummy),
                Times.Exactly(2));
    }

    [Test]
    public void Test_insertions_under()
    {
        // arrange
        var version1 = new ViSpanVersion(1);

        var adornment1 = new Mock<IViAdornment>().Object;

        var creator = CreateCreator(adornment1);
            
        var textView = new Mock<IWpfTextView>().Object;

        var adornmentInformation1 = new List<IAdornmentInformation>()
        {
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(10, 30)
                .WithVersion(version1)
                .Build(),
        };

        var adornmentInformation2 = new List<IAdornmentInformation>()
        {
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(0, 10)
                .WithVersion(version1)
                .Build(),
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(10, 30)
                .WithVersion(version1)
                .Build(),
        };

        var snapshotHistory = new SnapshotHistoryBuilder()
            .AddVersion(version1)
            .Build();

        var sut = CreateSut(creator, snapshotHistory);
        // act
        var result = sut.GetOrCreate(adornmentInformation1.ToArray(), textView, this._documentAdornmentControllerDummy);
        var result2 = sut.GetOrCreate(adornmentInformation2.ToArray(), textView, this._documentAdornmentControllerDummy);

        // assert
        result.Should().HaveCount(1);
        result2.Should().HaveCount(2);

        result2.First().Should().Be(adornment1);

        Mock.Get(creator)
            .Verify(
                adornmentCreator => adornmentCreator.Create(
                    It.IsAny<IAdornmentInformation>(),
                    textView,
                    It.IsAny<ISnapshotHistory>(),
                    this._documentAdornmentControllerDummy),
                Times.Exactly(2));
    }

    [Test]
    public void ReplaceAbove()
    {
        // arrange
        var version1 = new ViSpanVersion(1);

        var adornment1 = new Mock<IViAdornment>().Object;

        var creator = CreateCreator(adornment1);
            
        var textView = new Mock<IWpfTextView>().Object;

        var adornmentInformation1 = new List<IAdornmentInformation>()
        {
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(0, 10)
                .WithVersion(version1)
                .Build(),
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(10, 30)
                .WithVersion(version1)
                .Build(),
        };

        var adornmentInformation2 = new List<IAdornmentInformation>()
        {
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(10, 30)
                .WithVersion(version1)
                .Build(),
        };

        var adornmentInformation3 = new List<IAdornmentInformation>()
        {
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(0, 10)
                .WithVersion(version1)
                .Build(),
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(10, 30)
                .WithVersion(version1)
                .Build(),
        };

        var snapshotHistory = new SnapshotHistoryBuilder()
            .AddVersion(version1)
            .Build();

        var sut = CreateSut(creator, snapshotHistory);

        // act
        var result = sut.GetOrCreate(adornmentInformation1.ToArray(), textView, this._documentAdornmentControllerDummy);
        var result2 = sut.GetOrCreate(adornmentInformation2.ToArray(), textView, this._documentAdornmentControllerDummy);
        var result3 = sut.GetOrCreate(adornmentInformation3.ToArray(), textView, this._documentAdornmentControllerDummy);

        // assert
        result.Should().HaveCount(2);
        result2.Should().HaveCount(1);
        result3.Should().HaveCount(2);

        result2.First().Should().Be(adornment1);

        Mock.Get(creator)
            .Verify(
                adornmentCreator => adornmentCreator.Create(
                    It.IsAny<IAdornmentInformation>(),
                    textView,
                    It.IsAny<ISnapshotHistory>(),
                    this._documentAdornmentControllerDummy),
                Times.Exactly(3));
    }

    [Test]
    public void ReplaceUnder()
    {
        // arrange
        var version1 = new ViSpanVersion(1);

        var adornment1 = new Mock<IViAdornment>().Object;

        var creator = CreateCreator(adornment1);
            
        var textView = new Mock<IWpfTextView>().Object;

        var adornmentInformation1 = new List<IAdornmentInformation>()
        {
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(0, 10)
                .WithVersion(version1)
                .Build(),
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(10, 30)
                .WithVersion(version1)
                .Build(),
        };

        var adornmentInformation2 = new List<IAdornmentInformation>()
        {
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(0, 10)
                .WithVersion(version1)
                .Build(),
        };

        var adornmentInformation3 = new List<IAdornmentInformation>()
        {
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(0, 10)
                .WithVersion(version1)
                .Build(),
            new AdornmentInformationBuilder()
                .WithObjectCreationSpan(10, 30)
                .WithVersion(version1)
                .Build(),
        };

        var snapshotHistory = new SnapshotHistoryBuilder()
            .AddVersion(version1)
            .Build();

        var sut = CreateSut(creator, snapshotHistory);

        // act
        var result = sut.GetOrCreate(adornmentInformation1.ToArray(), textView, this._documentAdornmentControllerDummy);
        var result2 = sut.GetOrCreate(adornmentInformation2.ToArray(), textView, this._documentAdornmentControllerDummy);
        var result3 = sut.GetOrCreate(adornmentInformation3.ToArray(), textView, this._documentAdornmentControllerDummy);

        // assert
        result.Should().HaveCount(2);
        result2.Should().HaveCount(1);
        result3.Should().HaveCount(2);

        result2.First().Should().Be(adornment1);

        Mock.Get(creator)
            .Verify(
                adornmentCreator => adornmentCreator.Create(
                    It.IsAny<IAdornmentInformation>(),
                    textView,
                    It.IsAny<ISnapshotHistory>(),
                    this._documentAdornmentControllerDummy),
                Times.Exactly(3));
    }

    private static ViAdornmentCacheCreator CreateSut(IViAdornmentCreator creator, ISnapshotHistory snapshotHistory) =>
        new ItemBuilder<ViAdornmentCacheCreator>()
            .With(p => p.Ctor.adornmentCreator.Value(creator))
            .With(p => p.Ctor.snapshotHistory.Value(snapshotHistory))
            .Build();

    private static IViAdornmentCreator CreateCreator(IViAdornment expected) =>
        new ItemBuilder<IViAdornmentCreator>()
            .With(p => p.Create.Value(expected))
            .Build();
}