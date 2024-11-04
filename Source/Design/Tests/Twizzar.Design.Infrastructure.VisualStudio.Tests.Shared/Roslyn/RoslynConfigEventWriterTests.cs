using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[TestFixture]
public class RoslynConfigEventWriterTests
{
    #region static fields and constants

    [Test]
    public void When_FixtureItemConfigurationStartedEvent_was_not_raise_do_nothing()
    {
        // arrange
        var sut = new RoslynConfigEventWriterBuilder()
            .Build(out var context);

        var memberChangedEvent = Build.New<FixtureItemMemberChangedEvent>();

        // act
        sut.Handle(memberChangedEvent);

        // assert
        var roslynConfigWriterMock = context.GetAsMoq(p => p.Ctor.roslynConfigWriter);
        roslynConfigWriterMock.Verify(
            writer => writer.UpdateConfigAsync(memberChangedEvent.FixtureItemId, memberChangedEvent.MemberConfiguration),
            Times.Never);
    }

    [Test]
    public void When_configuration_has_started_then_configWriter_is_called()
    {
        // act
        var sut = new RoslynConfigEventWriterBuilder()
            .Build(out var context);

        var id = TestHelper.RandomNamedFixtureItemId();

        var startedEvent = new ItemBuilder<FixtureItemConfigurationStartedEvent>()
            .With(p => p.Ctor.FixtureItemId.Value(id))
            .Build();

        var memberChangedEvent = new ItemBuilder<FixtureItemMemberChangedEvent>()
            .With(p => p.Ctor.FixtureItemId.Value(id))
            .With(p => p.Ctor.IsFromInitialization.Value(false))
            .Build();
            
        // act
        sut.Handle(startedEvent);
        sut.Handle(memberChangedEvent);

        // assert
        context.Verify(p => p.Ctor.roslynConfigWriter.UpdateConfigAsync)
            .WhereIdIs(id)
            .WhereMemberConfigurationIs(memberChangedEvent.MemberConfiguration)
            .Called(1);
    }

    private class RoslynConfigEventWriterBuilder : ItemBuilder<RoslynConfigEventWriter, RoslynConfigEventWriterBuilderPaths>
    {

    }

    #endregion
}