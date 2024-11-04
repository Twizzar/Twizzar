using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace TestCreation.Services;

/// <inheritdoc cref="ITemplateService"/>
public class TemplateService : ITemplateService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateService"/> class.
    /// </summary>
    /// <param name="templateFileQuery"></param>
    public TemplateService(ITemplateFileQuery templateFileQuery)
    {
        EnsureHelper.GetDefault.Parameter(templateFileQuery, nameof(templateFileQuery))
            .ThrowWhenNull();
    }

    #region members

    /// <inheritdoc />
    public Task<CreationContext> AddTemplate(CreationContext source, CreationContext destination, ITemplateFile file)
    {
        var additionalUsings = GetAllRelevantNamespaces(source.SourceMember)
            .Prepend(source.Info.Namespace)
            .ToImmutableHashSet();

        var context = new TemplateContext(
            source,
            destination,
            file,
            additionalUsings);

        return Task.FromResult(destination with { TemplateContext = context });
    }

    private static IEnumerable<string> GetAllRelevantNamespaces(IMemberDescription memberDescription) =>
        memberDescription switch
        {
            IMethodDescription methodDescription => methodDescription.DeclaredParameters
                .Select(description => description.GetReturnTypeDescription())
                .SelectMany(GetNamespaces)
                .Concat(GetNamespaces(methodDescription.GetReturnTypeDescription())),
            IPropertyDescription propertyDescription =>
                GetNamespaces(propertyDescription.GetReturnTypeDescription()),
            _ => Enumerable.Empty<string>(),
        };

    private static IEnumerable<string> GetNamespaces(ITypeDescription typeDescription)
    {
        yield return typeDescription.TypeFullName.GetNameSpace();

        foreach (var (_, parameterType) in typeDescription.GenericTypeArguments)
        {
            if (parameterType.TypeFullName.IsSome)
            {
                yield return parameterType.TypeFullName.GetValueUnsafe().GetNameSpace();
            }
        }
    }

    #endregion
}