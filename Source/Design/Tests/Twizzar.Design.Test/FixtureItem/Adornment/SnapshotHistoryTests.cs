using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using Twizzar.Design.Test.Builder;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Test.FixtureItem.Adornment;

[TestClass()]
public class SnapshotHistoryTests
{
    [TestMethod()]
    public void Add_and_retrieving_the_same_entry_results_in_the_same_version()
    {
        // arrange
        var viVersion = new ViSpanVersion(1);

        var snapshot = new TextSnapshotBuilder()
            .WithVersion(new TextVersionBuilder().WithVersion(1).Build())
            .Build();

        var sut = new SnapshotHistory();

        // act
        sut.Add(snapshot);

        // assert
        sut.Get(viVersion).Should().Be(Maybe.Some(snapshot));
        sut.CurrentSnapshot.Should().Be(snapshot);
    }

    [TestMethod]
    public void History_only_has_hundred_entries()
    {
        // arrange
        static (IViSpanVersion version, ITextSnapshot snapshot) CreateEntry(int version) =>
            (new ViSpanVersion(version), 
                new TextSnapshotBuilder()
                    .WithVersion(new TextVersionBuilder().WithVersion(version).Build())
                    .Build());

        var entries = Enumerable.Range(0, 101)
            .Select(CreateEntry)
            .ToList();

        var sut = new SnapshotHistory();

        // act
        entries.ForEach(tuple => sut.Add(tuple.snapshot));

        // assert
        entries.Count.Should().Be(101);
        sut.Get(entries.First().version).Should().Be(Maybe.None<ITextSnapshot>());

        var (viSpanVersion, snapshot) = entries.Last();
        sut.Get(viSpanVersion).Should().Be(Maybe.Some(snapshot));
    }
}