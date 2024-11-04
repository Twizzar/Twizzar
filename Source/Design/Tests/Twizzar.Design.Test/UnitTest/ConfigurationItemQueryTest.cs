using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Core.Query.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class ConfigurationItemQueryTest
{
    private Mock<ISystemDefaultService> _systemDefaultService;
    private Mock<IConfigurationItemCacheQuery> _cacheQuery;
    private FixtureItemId _id;
    private Mock<ITypeDescription> _typeDesc;
    private Mock<IConfigurationItem> _configItem;
    private readonly NoneValue _cached = Maybe.None();
    private readonly string _errorMessage = "failed";

    [TestInitialize]
    public void Setup()
    {
        this._systemDefaultService = new Mock<ISystemDefaultService>();
        this._cacheQuery = new Mock<IConfigurationItemCacheQuery>();

        this._id = FixtureItemId.CreateNameless(TypeFullName.Create("fullName"));
        this._typeDesc = new Mock<ITypeDescription>();

        this._configItem = new Mock<IConfigurationItem>();
    }

    [TestMethod]
    public void Ctor_throws_exception_when_input_is_null()
    {
        Verify.Ctor<ConfigurationItemQuery>()
            .SetupParameter("message", TestHelper.RandomString())
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void GetConfigurationItem_throws_exception_when_parameter_is_null()
    {
        // arrange
        var sut = new ConfigurationItemQuery(
            new Mock<ISystemDefaultService>().Object,
            new Mock<IConfigurationItemCacheQuery>().Object);

        // act
        Func<Task> act1 = () => sut.GetConfigurationItem(null, new Mock< ITypeDescription>().Object);
        Func<Task> act2 = () => sut.GetConfigurationItem(FixtureItemId.CreateNameless(TypeFullName.Create("fullName")), null);

        // assert
        act1.Should().Throw<ArgumentNullException>();
        act2.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public async Task GetConfigurationItem_returns_valid_value()
    {
        // arrange
        this.SetupDefaultService(Result.Success(this._configItem.Object));
        this.SetupCacheQuery(this._cached);
        this.SetupConfigItem(Result.Success(this._configItem.Object));

        var sut = this.GetSut();

        // act
        var result = await sut.GetConfigurationItem(this._id, this._typeDesc.Object);

        // assert
        Assert.AreEqual(this._configItem.Object, result);
        this._systemDefaultService.Verify(service => service.GetDefaultConfigurationItem(this._typeDesc.Object, this._id.RootItemPath), Times.Once);
        this._cacheQuery.Verify(cache => cache.GetCached(this._id), Times.Once);
        this._configItem.Verify(config => config.Merge(this._cached), Times.Once);
    }

    [TestMethod]
    public void GetConfigurationItem_throws_exception_when_default_fails()
    {
        // arrange
        this.SetupDefaultService(new InvalidTypeDescriptionFailure(this._typeDesc.Object, this._errorMessage));

        var sut = this.GetSut();

        // act
        Func<Task> act = async () => await sut.GetConfigurationItem(this._id, this._typeDesc.Object);

        // assert
        act.Should().Throw<InvalidTypeDescriptionException>().WithMessage(this._errorMessage);
    }

    [TestMethod]
    public void GetConfigurationItem_throws_exception_when_merge_fails()
    {
        // arrange
        this.SetupDefaultService(Result.Success(this._configItem.Object));
        this.SetupCacheQuery(this._cached);
        this.SetupConfigItem(new InvalidConfigurationFailure(this._configItem.Object, this._errorMessage));

        var sut = this.GetSut();

        // act
        Func<Task> act = async () => await sut.GetConfigurationItem(this._id, this._typeDesc.Object);

        // assert
        act.Should().Throw<InvalidConfigurationException>().WithMessage(this._errorMessage);
    }

    private void SetupConfigItem(Result<IConfigurationItem, InvalidConfigurationFailure> result)
    {
        this._configItem.Setup(conf => conf.Merge(this._cached))
            .Returns(result);
    }

    private void SetupCacheQuery(Maybe<IConfigurationItem> configItem)
    {
        this._cacheQuery.Setup(cache => cache.GetCached(this._id)).Returns(configItem);
    }

    private void SetupDefaultService(Result<IConfigurationItem, InvalidTypeDescriptionFailure> result)
    {
        this._systemDefaultService.Setup(
                service => service.GetDefaultConfigurationItem(this._typeDesc.Object, this._id.RootItemPath))
            .Returns(result);
    }

    private ConfigurationItemQuery GetSut() => new(this._systemDefaultService.Object, this._cacheQuery.Object);

}