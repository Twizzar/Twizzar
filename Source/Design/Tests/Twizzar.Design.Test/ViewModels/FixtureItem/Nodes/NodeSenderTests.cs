using System.Collections.Immutable;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.TestCommon;
using Twizzar.TestCommon.Builder;
using Twizzar.TestCommon.TypeDescription.Builders;
using TwizzarInternal.Fixture;
using ViCommon.EnsureHelper;
using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.Design.TestCommon.DesignTestHelper;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.ViewModels.FixtureItem.Nodes;

[TestClass]
public class NodeSenderTests
{
    #region static fields and constants

    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

    #endregion

    #region members

    [TestMethod]
    public void All_constructor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<FixtureItemNodeSender>()
            .SetupParameter("message", RandomString())
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public async Task GetDefaultConfig_calls_correct_systemDefaultService_method_for_baseType()
    {
        // arrange
        var systemDefaultService = new Mock<ISystemDefaultService>();

        var memberConfiguration = Mock.Of<IMemberConfiguration>(configuration =>
            configuration.WithSource(It.IsAny<IConfigurationSource>()) == Mock.Of<IMemberConfiguration>());

        systemDefaultService
            .Setup(service => service.GetBaseTypeMemberConfigurationItem(It.IsAny<IBaseDescription>()))
            .Returns(memberConfiguration);

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            new TypeDescriptionBuilder().AsBaseType().Build(),
            new Mock<IMemberConfiguration>().Object);

        var sut = new FixtureItemNodeSender(
            new Mock<ICommandBus>().Object,
            systemDefaultService.Object);

        // act
        await sut.ChangeValue(
            Build.New<IFixtureItemNode>(),
            new ViEmptyToken(),
            information,
            Maybe.None());

        // assert
        systemDefaultService.Verify(
            service => service.GetBaseTypeMemberConfigurationItem(It.IsAny<IBaseDescription>()),
            Times.Once);
    }

    [TestMethod]
    public async Task GetDefaultConfig_calls_correct_systemDefaultService_method_for_parameter()
    {
        // arrange
        var systemDefaultService = new Mock<ISystemDefaultService>();

        var memberConfiguration = Mock.Of<IMemberConfiguration>(configuration =>
            configuration.WithSource(It.IsAny<IConfigurationSource>()) == Mock.Of<IMemberConfiguration>());

        systemDefaultService
            .Setup(
                service => service.GetDefaultConstructorParameterMemberConfigurationItem(
                    It.IsAny<IParameterDescription>(),
                    It.IsAny<Maybe<string>>()))
            .Returns(memberConfiguration);

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            new ParameterDescriptionBuilder().Build(),
            new Mock<IMemberConfiguration>().Object);

        var sut = new FixtureItemNodeSender(
            new Mock<ICommandBus>().Object,
            systemDefaultService.Object);

        // act
        await sut.ChangeValue(
            Build.New<IFixtureItemNode>(),
            new Mock<IViDefaultKeyword>().Object,
            information,
            Maybe.None());

        // assert
        systemDefaultService.Verify(
            service => service.GetDefaultConstructorParameterMemberConfigurationItem(
                It.IsAny<IParameterDescription>(),
                It.IsAny<Maybe<string>>()),
            Times.Once);
    }

    [TestMethod]
    public async Task ChangeValue_for_ctor_does_not_send_a_ChangeMemberConfigurationCommand()
    {
        // arrange
        var systemDefaultService = new Mock<ISystemDefaultService>();
        var commandBus = new Mock<ICommandBus>();

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            new TypeDescriptionBuilder().AsClass().Build(),
            new CtorMemberConfiguration(
                ImmutableArray<IMemberConfiguration>.Empty
                    .Add(new NullValueMemberConfiguration(RandomString(), Source)),
                ImmutableArray<ITypeFullName>.Empty,
                Source));

        var sut = new FixtureItemNodeSender(
            commandBus.Object,
            systemDefaultService.Object);

        var logger = new Mock<ILogger>();
        sut.Logger = logger.Object;

        // act
        await sut.ChangeValue(
            Build.New<IFixtureItemNode>(),
            new ViEmptyToken(),
            information,
            Maybe.None());

        // assert
        logger.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Once);
        commandBus.Verify(bus => bus.SendAsync(It.IsAny<ChangeMemberConfigurationCommand>()), Times.Never);
    }

    [TestMethod]
    public async Task ChangeValue_with_same_config_not_send_a_command()
    {
        // arrange
        var systemDefaultService = new Mock<ISystemDefaultService>();
        var commandBus = new Mock<ICommandBus>();
        var value = RandomString();
        var config = new ValueMemberConfiguration(RandomString(), value, new FromUserInterface());

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            new TypeDescriptionBuilder().AsBaseType().Build(),
            config);

        var sut = new FixtureItemNodeSender(
            commandBus.Object,
            systemDefaultService.Object);

        var logger = new Mock<ILogger>();
        sut.Logger = logger.Object;

        // act
        await sut.ChangeValue(
            Build.New<IFixtureItemNode>(),
            ViStringToken.CreateWithoutWhitespaces(0, 0, $"\"{value}\""),
            information,
            Maybe.None());

        // assert
        commandBus.Verify(bus => bus.SendAsync(It.IsAny<ChangeMemberConfigurationCommand>()), Times.Never);
    }

    [TestMethod]
    public async Task GetDefaultConfig_calls_correct_systemDefaultService_method_for_a_member()
    {
        // arrange
        var systemDefaultService = new Mock<ISystemDefaultService>();

        var memberConfiguration = Mock.Of<IMemberConfiguration>(configuration =>
            configuration.WithSource(It.IsAny<IConfigurationSource>()) == Mock.Of<IMemberConfiguration>());

        systemDefaultService
            .Setup(
                service => service.GetDefaultMemberConfigurationItem(
                    It.IsAny<IMemberDescription>(),
                    It.IsAny<Maybe<string>>()))
            .Returns(memberConfiguration);

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            Mock.Of<IMemberDescription>(description => description.Name == ""),
            Mock.Of<IMemberConfiguration>());

        var sut = new FixtureItemNodeSender(
            new Mock<ICommandBus>().Object,
            systemDefaultService.Object);

        // act
        await sut.ChangeValue(
            Build.New<IFixtureItemNode>(),
            new ViEmptyToken(),
            information,
            Maybe.None());

        // assert
        systemDefaultService.Verify(
            service => service.GetDefaultMemberConfigurationItem(
                It.IsAny<IMemberDescription>(),
                It.IsAny<Maybe<string>>()),
            Times.Once);
    }

    [TestMethod]
    public async Task NullKeywordToken_is_converted_to_NullValueMemberConfiguration()
    {
        // arrange
        var token = new Mock<IViNullKeywordToken>().Object;
        var memberName = RandomString();
        var inputConfig = new Mock<IMemberConfiguration>();

        inputConfig
            .Setup(configuration => configuration.Name)
            .Returns(memberName);

        // act
        var result = await ChangeValue(token, inputConfig.Object);

        // assert
        var memberConfig = result.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IMemberConfiguration>>()
            .Subject.Value;

        memberConfig.Should().BeAssignableTo<NullValueMemberConfiguration>();
        memberConfig.Name.Should().Be(memberName);
    }

    [TestMethod]
    public async Task StringToken_is_converted_to_ValueMemberConfiguration()
    {
        // arrange
        var text = RandomString();

        var token = new Mock<IViStringToken>();

        token.Setup(t => t.Text)
            .Returns(text);

        var memberName = RandomString();
        var inputConfig = new Mock<IMemberConfiguration>();

        inputConfig
            .Setup(configuration => configuration.Name)
            .Returns(memberName);

        // act
        var result = await ChangeValue(token.Object, inputConfig.Object);

        // assert
        var memberConfig = result.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IMemberConfiguration>>()
            .Subject.Value;

        var valueMemberConfig =
            memberConfig.Should().BeAssignableTo<ValueMemberConfiguration>().Subject;

        valueMemberConfig.Name.Should().Be(memberName);
        valueMemberConfig.Value.Should().Be(text);
    }

    [TestMethod]
    [Ignore("Fix during UI adjustments for builder.")]
    public async Task CharToken_is_converted_to_ValueMemberConfiguration()
    {
        // TODO: fix during UI adjustments for Builder.
        // arrange
        var character = 'c';

        var token = new Mock<IViCharToken>();

        token.Setup(t => t.Character)
            .Returns(character);

        var memberName = RandomString();
        var inputConfig = new Mock<IMemberConfiguration>();

        inputConfig
            .Setup(configuration => configuration.Name)
            .Returns(memberName);

        // act
        var result = await ChangeValue(token.Object, inputConfig.Object);

        // assert
        var memberConfig = result.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IMemberConfiguration>>()
            .Subject.Value;

        var valueMemberConfig =
            memberConfig.Should().BeAssignableTo<ValueMemberConfiguration>().Subject;

        valueMemberConfig.Name.Should().Be(memberName);
        var value = valueMemberConfig.Value.Should().BeAssignableTo<char>().Subject;
        value.Should().Be(character);
    }

    [TestMethod]
    public async Task NumericToken_is_converted_to_ValueMemberConfiguration()
    {
        // arrange
        var randomNumeric = GetRandomNumericWithSuffix(Maybe.None());

        var token = new Mock<IViNumericToken>();

        token.Setup(t => t.NumericWithSuffix)
            .Returns(randomNumeric);

        var memberName = RandomString();
        var inputConfig = new Mock<IMemberConfiguration>();

        inputConfig
            .Setup(configuration => configuration.Name)
            .Returns(memberName);

        // act
        var result = await ChangeValue(token.Object, inputConfig.Object);

        // assert
        var memberConfig = result.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IMemberConfiguration>>()
            .Subject.Value;

        var valueMemberConfig =
            memberConfig.Should().BeAssignableTo<ValueMemberConfiguration>().Subject;

        valueMemberConfig.Name.Should().Be(memberName);
        valueMemberConfig.Value.Should().Be(new SimpleLiteralValue(randomNumeric));
    }

    [TestMethod]
    public async Task BoolToken_is_converted_to_ValueMemberConfiguration()
    {
        // arrange
        var boolean = RandomBool();

        var token = new Mock<IViBoolToken>();

        token.Setup(t => t.Boolean)
            .Returns(boolean);

        var memberName = RandomString();
        var inputConfig = new Mock<IMemberConfiguration>();

        inputConfig
            .Setup(configuration => configuration.Name)
            .Returns(memberName);

        // act
        var result = await ChangeValue(token.Object, inputConfig.Object);

        // assert
        var memberConfig = result.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IMemberConfiguration>>()
            .Subject.Value;

        var valueMemberConfig =
            memberConfig.Should().BeAssignableTo<ValueMemberConfiguration>().Subject;

        valueMemberConfig.Name.Should().Be(memberName);
        valueMemberConfig.Value.Should().Be(new SimpleLiteralValue(boolean));
    }

    [TestMethod]
    public async Task LinkToken_is_converted_to_LinkMemberConfiguration()
    {
        // arrange
        var typeName = RandomDesignTypeFullName();
        var linkName = RandomString();

        var typeToken = new Mock<IViTypeToken>();

        typeToken
            .Setup(t => t.TypeFullNameToken)
            .Returns(typeName.GetTypeFullNameToken);

        var linkNameToken = new Mock<IViLinkNameToken>();

        linkNameToken
            .Setup(t => t.Name)
            .Returns(linkName);

        var token = new Mock<IViLinkToken>();

        token
            .Setup(t => t.TypeToken)
            .Returns(Maybe.Some(typeToken.Object));

        var memberName = RandomString();
        var inputConfig = new Mock<IMemberConfiguration>();

        inputConfig
            .Setup(configuration => configuration.Name)
            .Returns(memberName);

        // act
        var result = await ChangeValue(token.Object, inputConfig.Object);

        // assert
        var memberConfig = result.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IMemberConfiguration>>()
            .Subject.Value;

        var linkMemberConfiguration =
            memberConfig.Should().BeAssignableTo<LinkMemberConfiguration>().Subject;

        linkMemberConfiguration.Name.Should().Be(memberName);

        linkMemberConfiguration.ConfigurationLink.Name.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<string>>();
    }

    [TestMethod]
    public async Task UndefinedKeywordToken_is_converted_to_UndefinedMemberConfiguration()
    {
        // arrange
        var token = new Mock<IViUndefinedKeywordToken>();
        var typeFullName = RandomString();

        var memberName = RandomString();
        var inputConfig = new Mock<IMemberConfiguration>();

        inputConfig
            .Setup(configuration => configuration.Name)
            .Returns(memberName);

        // act
        var result = await ChangeValue(token.Object, inputConfig.Object, typeFullName);

        // assert
        var memberConfig = result.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IMemberConfiguration>>()
            .Subject.Value;

        var config =
            memberConfig.Should().BeAssignableTo<UndefinedMemberConfiguration>().Subject;

        config.Name.Should().Be(memberName);
        config.Type.FullName.Should().Be(typeFullName);
    }

    [TestMethod]
    public async Task UniqueKeywordToken_is_converted_to_UniqueValueMemberConfiguration()
    {
        // arrange
        var token = new Mock<IViUniqueKeywordToken>();

        var memberName = RandomString();
        var inputConfig = new Mock<IMemberConfiguration>();

        inputConfig
            .Setup(configuration => configuration.Name)
            .Returns(memberName);

        // act
        var result = await ChangeValue(token.Object, inputConfig.Object);

        // assert
        var memberConfig = result.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IMemberConfiguration>>()
            .Subject.Value;

        memberConfig.Should().BeAssignableTo<UniqueValueMemberConfiguration>();

        memberConfig.Name.Should().Be(memberName);
    }

    [TestMethod]
    public async Task ViEnumToken_is_converted_to_ValueMemberConfiguration()
    {
        // arrange
        var token = Mock.Of<IViEnumToken>(
            enumToken =>
                enumToken.EnumName == "EnumName" &&
                enumToken.EnumType == Maybe.Some("test"));

        var memberName = RandomString();
        var inputConfig = new Mock<IMemberConfiguration>();

        inputConfig
            .Setup(configuration => configuration.Name)
            .Returns(memberName);

        // act
        var result = await ChangeValue(token, inputConfig.Object);

        // assert
        var memberConfig = result.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IMemberConfiguration>>()
            .Subject.Value;

        var config =
            memberConfig.Should().BeAssignableTo<ValueMemberConfiguration>().Subject;

        config.Should().BeAssignableTo<ValueMemberConfiguration>();
        config.Name.Should().Be(memberName);
        config.DisplayValue.Should().Be(token.EnumType.GetValueUnsafe() + "." + token.EnumName);
    }

    [TestMethod]
    public async Task CtorToken_does_not_update_the_config()
    {
        // arrange
        var token = new Mock<IViCtorToken>();

        // act
        var result = await ChangeValue(token.Object);

        // assert
        result.IsNone.Should().BeTrue();
    }

    [TestMethod]
    public async Task InvalidToken_does_not_update_the_config()
    {
        // arrange
        var token = new Mock<IViInvalidToken>();

        // act
        var result = await ChangeValue(token.Object);

        // assert
        result.IsNone.Should().BeTrue();
    }

    [TestMethod]
    public async Task UpdateMemberConfig_also_updates_parent()
    {
        // arrange
        var systemDefaultService = new Mock<ISystemDefaultService>();
        var newMemberConfig = new Mock<IMemberConfiguration>().Object;
        var parent = new Mock<IFixtureItemNode>();

        var parentMemberConfig = new Mock<IMemberConfiguration>();

        parentMemberConfig
            .Setup(configuration => configuration.Name)
            .Returns(RandomString());

        parent.Setup(node => node.FixtureItemInformation)
            .Returns(
                new FixtureItemInformation(
                    RandomNamelessFixtureItemId(),
                    RandomString(),
                    Build.New<ITypeDescription>(),
                    parentMemberConfig.Object));

        systemDefaultService
            .Setup(service => service.GetBaseTypeMemberConfigurationItem(It.IsAny<IBaseDescription>()))
            .Returns(Build.New<IMemberConfiguration>());

        var information = new FixtureItemInformation(
            RandomNamelessFixtureItemId(),
            RandomString(),
            new TypeDescriptionBuilder().AsBaseType().Build(),
            Build.New<IMemberConfiguration>());

        var sut = new FixtureItemNodeSender(
            Build.New<ICommandBus>(),
            Build.New<ISystemDefaultService>());

        // act
        await sut.UpdateMemberConfigAsync(
            Build.New<IFixtureItemNode>(),
            newMemberConfig,
            information,
            Maybe.Some(parent.Object));

        // assert
        parent.Verify(node => node.CommitMemberConfig(It.IsAny<IMemberConfiguration>()), Times.Once);
    }

    [TestMethod]
    public async Task UpdateMemberConfig_also_updates_all_siblings()
    {
        // arrange
        var systemDefaultService = new Mock<ISystemDefaultService>();
        var newMemberConfig = new Mock<IMemberConfiguration>().Object;
        var parent = new Mock<IFixtureItemNode>();

        var parentMemberConfig = new Mock<IMemberConfiguration>();

        var sibling = new Mock<IFixtureItemNode>();

        sibling.Setup(node => node.FixtureItemInformation)
            .Returns(
                new FixtureItemInformation(
                    RandomNamelessFixtureItemId(),
                    RandomString(),
                    Build.New<ITypeDescription>(),
                    Build.New<IMemberConfiguration>()));

        parentMemberConfig
            .Setup(configuration => configuration.Name)
            .Returns(RandomString());

        parent
            .Setup(node => node.FixtureItemInformation)
            .Returns(
                new FixtureItemInformation(
                    RandomNamelessFixtureItemId(),
                    RandomString(),
                    Build.New<ITypeDescription>(),
                    parentMemberConfig.Object));

        parent
            .Setup(node => node.Children)
            .Returns(new[] { sibling.Object });

        systemDefaultService
            .Setup(service => service.GetBaseTypeMemberConfigurationItem(It.IsAny<IBaseDescription>()))
            .Returns(Build.New<IMemberConfiguration>());

        var information = new FixtureItemInformation(
            RandomNamelessFixtureItemId(),
            RandomString(),
            new TypeDescriptionBuilder().AsBaseType().Build(),
            Build.New<IMemberConfiguration>());

        var sut = new FixtureItemNodeSender(
            Build.New<ICommandBus>(),
            Build.New<ISystemDefaultService>());

        // act
        await sut.UpdateMemberConfigAsync(
            Build.New<IFixtureItemNode>(),
            newMemberConfig,
            information,
            Maybe.Some(parent.Object));

        // assert
        sibling.Verify(node => node.UpdateFixtureItemId(It.IsAny<FixtureItemId>()), Times.Once);
    }

    static async Task<Maybe<IMemberConfiguration>> ChangeValue(
        IViToken token,
        IMemberConfiguration memberConfiguration = null,
        string fullName = "test")
    {
        var typeFullName = new TypeFullNameBuilder(fullName).Build();
        memberConfiguration ??= new Mock<IMemberConfiguration>().Object;

        var systemDefaultService = new Mock<ISystemDefaultService>();

        systemDefaultService
            .Setup(service => service.GetBaseTypeMemberConfigurationItem(It.IsAny<IBaseDescription>()))
            .Returns(new Mock<IMemberConfiguration>().Object);

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            new TypeDescriptionBuilder().AsBaseType().WithTypeFullName(typeFullName).Build(),
            memberConfiguration);

        var commandBusSpy = new CommandBusSpy();

        var sut = new FixtureItemNodeSender(
            commandBusSpy,
            systemDefaultService.Object);

        // act
        await sut.ChangeValue(
            Build.New<IFixtureItemNode>(),
            token,
            information,
            Maybe.None());

        // assert
        return commandBusSpy.MemberConfiguration;
    }

    #endregion

    #region Nested type: CommandBusSpy

    public class CommandBusSpy : ICommandBus
    {
        #region properties

        public Maybe<IMemberConfiguration> MemberConfiguration { get; private set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public int RunningCommands { get; } = 1;

        #endregion

        #region members

        /// <inheritdoc />
        public Task SendAsync<TCommand>(ICommand<TCommand> command)
            where TCommand : ICommand
        {
            if (command is ChangeMemberConfigurationCommand x)
            {
                this.MemberConfiguration = Maybe.Some(x.MemberConfiguration);
            }

            return Task.CompletedTask;
        }

        #endregion
    }

    #endregion
}