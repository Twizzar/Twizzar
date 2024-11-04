using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.CoreInterfaces.Command.Commands;

/// <summary>
/// Command for creating an Unit test automatically.
/// </summary>
[ExcludeFromCodeCoverage]
public record CreateUnitTestCommand(string FilePath, int CharOffset, int CurrentColumn, int CurrentLine) : ICommand<CreateUnitTestCommand>;