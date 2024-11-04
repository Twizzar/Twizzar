using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.CoreInterfaces.Command.Commands;

/// <summary>
/// Command to create a custom builder.
/// </summary>
/// <param name="DocumentWriter"></param>
/// <param name="BuilderName"></param>
/// <param name="AdornmentInformation"></param>
[ExcludeFromCodeCoverage]
public record CreateCustomBuilderCommand(
    IDocumentWriter DocumentWriter,
    string BuilderName,
    IAdornmentInformation AdornmentInformation) : ICommand<CreateCustomBuilderCommand>;