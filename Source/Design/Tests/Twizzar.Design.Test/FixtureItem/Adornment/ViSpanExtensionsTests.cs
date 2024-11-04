using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Common.FixtureItem.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using Twizzar.Design.Test.Builder;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.FixtureItem.Adornment;

[TestClass]
public class ViSpanExtensionsTests
{
    [TestMethod]
    public void ToSpanTest()
    {
        // arrange
        var start = RandomInt(0, int.MaxValue / 2);
        var length = RandomInt(0, int.MaxValue / 2);
        var viSpan = new ViSpan(start, length);

        // act
        var span = viSpan.ToSpan();

        // assert
        span.Should().BeOfType<Span>();
        span.Start.Should().Be(start);
        span.Length.Should().Be(length);
    }

    [TestMethod]
    public void ToSnapshotSpanTest()
    {
        // arrange
        var start = RandomInt(0, int.MaxValue / 2);
        var length = RandomInt(0, int.MaxValue / 2);
        var version = new ViSpanVersion(1);
        var viSpan = new ViSpan(start, length, version);
        var expectedSnapshot = new TextSnapshotBuilder()
            .WithLength(int.MaxValue)
            .Build();
        var snapshotHistory = new SnapshotHistoryBuilder()
            .AddVersion(version, expectedSnapshot)
            .Build();

        // act
        var snapshotSpan = viSpan.ToSnapshotSpan(snapshotHistory);

        // assert
        snapshotSpan.Should().BeOfType<SnapshotSpan>();
        snapshotSpan.Span.Start.Should().Be(start);
        snapshotSpan.Length.Should().Be(length);
        snapshotSpan.Snapshot.Should().Be(expectedSnapshot);
    }

    [TestMethod]
    public void ToSnapshotSpan_throws_InternalError_when_version_not_found()
    {
        // arrange
        var start = RandomInt(0, int.MaxValue / 2);
        var length = RandomInt(0, int.MaxValue / 2);
        var viSpan = new ViSpan(start, length, new ViSpanVersion(2));
        var expectedSnapshot = new TextSnapshotBuilder()
            .WithLength(int.MaxValue)
            .Build();
        var snapshotHistory = new SnapshotHistoryBuilder()
            .AddVersion(new ViSpanVersion(1), expectedSnapshot)
            .Build();

        // act
        Action action = () => viSpan.ToSnapshotSpan(snapshotHistory);

        // assert
        action.Should().Throw<InternalException>();
    }
}