using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EnvDTE;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Twizzar.Design.Infrastructure.VisualStudio.Services;

using TwizzarInternal.Fixture;

#pragma warning disable VSTHRD010

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Services;

[TestFixture]
public class ShortcutServiceTests
{

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ShortcutService>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task SetDefaultKeyBindingsAsync_sets_binding_for_twizzar_command_when_binding_is_not_default()
    {
        // arrange
        var commands = new Mock<Commands>();
        var commandForCheck = CreateCommand("TWIZZAR.About", "Global::Ctrl+Alt+N, Ctrl+Alt+B");
        var commandsList = new List<EnvDTE.Command>()
        {
            commandForCheck,
            CreateCommand("TWIZZAR.OpenOrClose", "Global::Ctrl+Alt+N, Ctrl+Alt+V"),
            CreateCommand("TWIZZAR.CreateUnitTest", "Global::Ctrl+Alt+N, Ctrl+Alt+N"),
            CreateCommand("TWIZZAR.UnitTestCodeNavigation", "Global::Ctrl+Alt+N, Ctrl+Alt+G"),
        };

        commands.Setup(c => c.GetEnumerator()).Returns(commandsList.GetEnumerator());
        var service = SetupService(commands);

        // act
        await service.SetDefaultKeyBindingsAsync();

        // assert
        (commandForCheck.Bindings as Array).Should().Contain("Global::Ctrl+Alt+N, Ctrl+Alt+A");
    }

    [Test]
    public async Task SetDefaultKeyBindingsAsync_reset_binding_for_non_twizzar_command()
    {
        // arrange
        var commands = new Mock<Commands>();
        var commandForCheck = CreateCommand("Some.Other_Command", "Global::Ctrl+Alt+N, Ctrl+Alt+V");
        var commandsList = new List<EnvDTE.Command>()
        {
            commandForCheck,
            CreateCommand("TWIZZAR.About", "Global::Ctrl+Alt+N, Ctrl+Alt+A"),
            CreateCommand("TWIZZAR.OpenOrClose", "Global::Ctrl+Alt+N, Ctrl+Alt+V"),
            CreateCommand("TWIZZAR.CreateUnitTest", "Global::Ctrl+Alt+N, Ctrl+Alt+N"),
            CreateCommand("TWIZZAR.UnitTestCodeNavigation", "Global::Ctrl+Alt+N, Ctrl+Alt+G"),
        };

        commands.Setup(c => c.GetEnumerator()).Returns(commandsList.GetEnumerator());
        var service = SetupService(commands);

        // act
        await service.SetDefaultKeyBindingsAsync();


        // assert
        (commandForCheck.Bindings as Array).Should().BeEmpty();
    }

    private static ShortcutService SetupService(Mock<Commands> commands)
    {
        var dte = new ItemBuilder<DTE>()
            .With(dte => dte.Commands.Value(commands.Object))
            .Build();

        var service = new ShortcutService(dte);
        return service;
    }

    private static EnvDTE.Command CreateCommand(string name, string shortcut)
    {
        var command = new Mock<EnvDTE.Command>();
        command.SetupAllProperties();

        command.Setup(c => c.Name).Returns(name);

        var commandObject = command.Object;

        commandObject.Bindings = string.IsNullOrWhiteSpace(shortcut)
            ? Array.Empty<object>()
            : new object[] { shortcut };

        return commandObject;
    }
}