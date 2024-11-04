using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <inheritdoc cref="IVariablesProviderFactory" />
[ExcludeFromCodeCoverage]
public class VariablesProviderFactory : IVariablesProviderFactory
{
    #region members

    /// <inheritdoc />
    public IVariablesProvider Create(ITemplateContext templateContext) =>
        new TemplateVariableProvider(templateContext);

    #endregion
}