using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <inheritdoc cref="ITemplateCodeProvider" />
public class TemplateCodeProvider : ITemplateCodeProvider
{
    #region fields

    private readonly ITemplateVisitor _visitor;
    private readonly IVariableReplacer _variableReplacer;
    private readonly IVariablesProviderFactory _variablesProviderFactory;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateCodeProvider"/> class.
    /// </summary>
    /// <param name="visitor"></param>
    /// <param name="variableReplacer"></param>
    /// <param name="variablesProviderFactory"></param>
    public TemplateCodeProvider(
        ITemplateVisitor visitor,
        IVariableReplacer variableReplacer,
        IVariablesProviderFactory variablesProviderFactory)
    {
        this._visitor = visitor;
        this._variableReplacer = variableReplacer;
        this._variablesProviderFactory = variablesProviderFactory;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public string GetCode(SnippetType type, ITemplateContext context)
    {
        var snipped = context.File.GetSingleSnipped(type);
        var code = this._visitor.Visit(snipped, context);
        var variablesProvider = this._variablesProviderFactory.Create(context);
        code = this._variableReplacer.ReplaceAll(code, variablesProvider);
        return code;
    }

    #endregion
}