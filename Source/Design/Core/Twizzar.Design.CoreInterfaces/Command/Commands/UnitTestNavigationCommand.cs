using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.CoreInterfaces.Command.Commands;

/// <summary>
/// Command for navigating from code to unit test and vise versa.
/// </summary>
[ExcludeFromCodeCoverage]
public record UnitTestNavigationCommand(string FilePath, int CharOffset, int CurrentColumn, int CurrentLine) : ICommand<UnitTestNavigationCommand>;