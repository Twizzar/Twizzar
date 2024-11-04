using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// All important services and data for creating a unit test.
/// </summary>
/// <param name="Info">Information for the test creation.</param>
/// <param name="SourceMember">The member where the user has started the interaction, for test creation this will always be the member under test.</param>
/// <param name="SourceType">The type of the source member.</param>
/// <param name="CodeAnalysisContext">Context for analyzing the code.</param>
/// <param name="TemplateContext">Context needed for getting code snippets form the template.</param>
public record CreationContext(
    CreationInfo Info,
    IMemberDescription SourceMember,
    ITypeDescription SourceType,
    ICodeAnalysisContext CodeAnalysisContext,
    Maybe<ITemplateContext> TemplateContext);