using System.Linq;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <inheritdoc cref="IVariableReplacer" />
public class VariableReplacer : IVariableReplacer
{
    #region members

    /// <inheritdoc />
    public string ReplaceAll(string code, IVariablesProvider variablesProvider) =>
        variablesProvider.Aggregate(code, Replace);

    private static string Replace(string input, TemplateVariable variable) =>
        input.Replace($"${variable.Tag}$", variable.Value);

    #endregion
}