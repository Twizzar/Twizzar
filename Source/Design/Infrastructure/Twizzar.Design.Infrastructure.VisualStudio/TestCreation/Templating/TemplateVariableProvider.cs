using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <inheritdoc cref="IVariablesProvider" />
[ExcludeFromCodeCoverage]
public class TemplateVariableProvider : IVariablesProvider
{
    #region fields

    private readonly ITemplateContext _templateContext;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateVariableProvider"/> class.
    /// </summary>
    /// <param name="templateContext"></param>
    public TemplateVariableProvider(ITemplateContext templateContext)
    {
        this._templateContext = templateContext;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public IEnumerator<TemplateVariable> GetEnumerator()
    {
        yield return new TemplateVariable("testNamespace", this._templateContext.TargetCreationContext.Info.Namespace);
        yield return new TemplateVariable("testClass", this._templateContext.TargetCreationContext.Info.Type);
        yield return new TemplateVariable("testMethod", this._templateContext.TargetCreationContext.Info.Member);
        yield return new TemplateVariable("namespaceUnderTest", this._templateContext.SourceCreationContext.Info.Namespace);
        yield return new TemplateVariable("typeUnderTest", this._templateContext.SourceCreationContext.SourceType.TypeFullName.GetFriendlyCSharpTypeName());
        yield return new TemplateVariable("memberUnderTest", this._templateContext.SourceCreationContext.Info.Member);

        var argumentNames = string.Empty;

        if (this._templateContext.SourceCreationContext.SourceMember is IMethodDescription methodDescription)
        {
            argumentNames = methodDescription.Parameters;
        }

        yield return new TemplateVariable("argumentNames", argumentNames);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion
}