using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;
using Twizzar.Design.Shared.Infrastructure.Discovery;

namespace Twizzar.Design.Shared.Infrastructure.Interfaces
{
    /// <summary>
    /// Service for discovering information in a syntax node.
    /// </summary>
    public interface IDiscoverer
    {
        /// <summary>
        /// Check if the node contains: <c>new ItemBuilder&lt;T&gt;();</c>.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>
        /// A tuple:
        /// ItemBuilderCreationInformation: the information about the itemBuilder.
        /// PathProviderInformation: Information for the PathProvider which needs to be created.
        /// </returns>
        IValuesOperation<(ItemBuilderCreationInformation ItemBuilderCreationInformation, PathProviderInformation PathProviderInformation)>
            DiscoverItemBuilderCreation(IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> operation);

        /// <summary>
        /// Check if the node contains <c>class MyBuilder : ItemBuilder&lt;T, TPathProvider&gt;</c>.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>Information for the PathProvider which needs to be created.</returns>
        IValuesOperation<PathProviderInformation> DiscoverCustomItemBuilder(IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> operation);

        /// <summary>
        /// Check if the node contains code like <c>p.Member.Value(3)</c>.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>
        /// A tuple:
        /// ItemBuilderCreationInformation: the information about the itemBuilder.
        /// PathProviderInformation: Information for the PathProvider which needs to be created.
        /// </returns>
        IValuesOperation<(IdentifierNameSyntax IdentifierNameSyntax, SemanticModel SemanticModel, PathProviderInformation PathProviderInformation)>
            DiscoverMemberSelection(IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> operation);
    }
}