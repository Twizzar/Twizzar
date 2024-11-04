using System;
using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Core.Query.Services;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class ConfigurationItemCacheQueryTest
{
    private Mock<IConfigurationItemFactory> _configItemFactory;
    private FixtureItemId _id;
    private EventMessage _eventMessageCreate;
    private EventMessage _eventMessageChanged;
    private Mock<IConfigurationItem> _configItem;
    private FixtureItemConfigurationEndedEvent _fixtureItemConfigurationEndedEvent;
    private Mock<IMemberConfiguration> _memberConfiguration;

    [TestInitialize]
    public void Setup()
    {
        this._configItemFactory = new Mock<IConfigurationItemFactory>();
        this._id = FixtureItemId.CreateNameless(TypeFullName.Create("fullName")).WithRootItemPath("rootItemPath");
        this._fixtureItemConfigurationEndedEvent = new FixtureItemConfigurationEndedEvent(this._id.RootItemPath.GetValueUnsafe());

        this._eventMessageCreate = EventMessage.Create(new FixtureItemCreatedEvent(this._id), typeof(FixtureItemCreatedEvent));
        this._memberConfiguration = new Mock<IMemberConfiguration>();
        this._eventMessageChanged = EventMessage.Create(new FixtureItemMemberChangedEvent(this._id, this._memberConfiguration.Object), typeof(FixtureItemMemberChangedEvent));
        this._configItem = new Mock<IConfigurationItem>();
    }

    [TestMethod]
    public void Ctor_throws_exception_when_parameter_is_null()
    {
        Verify.Ctor<ConfigurationItemCacheQuery>()
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void Synchronize_adds_elements_to_cache()
    {
        // arrange 
        this.SetupFactory();
        var sut = new ConfigurationItemCacheQuery(this._configItemFactory.Object);

        // act
        sut.Synchronize(this._eventMessageCreate);
        var result = sut.GetCached(this._id);

        // assert
        Assert.IsTrue(result.IsSome);
    }

    [TestMethod]
    public void Synchronize_adds_elements_with_member_changed_to_cache()
    {
        // arrange 
        this.SetupFactory(ImmutableDictionary.Create<string, IMemberConfiguration>());
        this.SetupConfigs();

        var sut = new ConfigurationItemCacheQuery(this._configItemFactory.Object);


        // act
        sut.Synchronize(this._eventMessageCreate);
        sut.Synchronize(this._eventMessageChanged);
        var result = sut.GetCached(this._id);

        // assert
        Assert.IsTrue(result.IsSome);
        this._configItem.Verify(conf => conf.WithMemberConfigurations(It.IsAny<IImmutableDictionary<string, IMemberConfiguration>>()), Times.Once);
    }

    [TestMethod]
    public void Synchronize_change_without_crete_throws_exception()
    {
        // arrange 
        var sut = new ConfigurationItemCacheQuery(this._configItemFactory.Object);

        // act
        Action act = () => sut.Synchronize(this._eventMessageChanged);

        // assert
        act.Should().Throw<InternalException>();
    }

    [TestMethod]
    public void Synchronize_adds_element_multiple_times_throws_exception()
    {
        // arrange 
        this.SetupFactory();
        var sut = new ConfigurationItemCacheQuery(this._configItemFactory.Object);


        // act
        sut.Synchronize(this._eventMessageCreate);
        Action act = () => sut.Synchronize(this._eventMessageCreate);
            
        // assert
        act.Should().Throw<InternalException>();
    }

    [TestMethod]
    public void Handle_event_store_cleared_with_empty_cache()
    {
        // arrange 
        var sut = new ConfigurationItemCacheQuery(this._configItemFactory.Object);

        // act
        sut.Handle(this._fixtureItemConfigurationEndedEvent);
        var result = sut.GetCached(this._id);

        // assert
        Assert.AreEqual(Maybe.None(), result);
    }

    [TestMethod]
    public void Handle_event_store_cleared_with_filled_cache_results_in_empty_cache()
    {
        // arrange 
        this.SetupFactory();
        var sut = new ConfigurationItemCacheQuery(this._configItemFactory.Object);

        // act
        sut.Synchronize(this._eventMessageCreate);
        var result = sut.GetCached(this._id);
            
        // assert
        Assert.IsTrue(result.IsSome);

        // act2
        sut.Handle(this._fixtureItemConfigurationEndedEvent);
        var result2 = sut.GetCached(this._id);

        // assert2
        Assert.AreEqual(Maybe.None(), result2);
    }

    private void SetupFactory(ImmutableDictionary<string, IMemberConfiguration> dict = null)
    {
        this._configItemFactory.Setup(
                fact => fact.CreateConfigurationItem(
                    this._id,
                    It.IsAny<IImmutableDictionary<string, IFixtureConfiguration>>(),
                    It.IsAny<IImmutableDictionary<string, IMemberConfiguration>>(),
                    It.IsAny<IImmutableDictionary<string , IImmutableList<object>>>()))
            .Returns(this._configItem.Object);

        if (dict != null)
        {
            this._configItem
                .Setup(conf => conf.MemberConfigurations)
                .Returns(dict);
        }
    }

    private void SetupConfigs()
    {
        this._configItem.Setup(conf =>
                conf.WithMemberConfigurations(It.IsAny<IImmutableDictionary<string, IMemberConfiguration>>()))
            .Returns(this._configItem.Object);

        this._memberConfiguration.Setup(conf => conf.Name).Returns("MemberConfig");
    }
}