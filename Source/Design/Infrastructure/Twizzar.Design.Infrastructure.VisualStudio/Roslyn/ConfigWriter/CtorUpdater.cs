using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.ConfigWriter
{
    /// <summary>
    /// Updates the <see cref="ConstructorDeclarationSyntax"/> an keeps track of the body statements so they can be replaced for the update.
    /// </summary>
    public class CtorUpdater
    {
        #region static fields and constants

        private static readonly IEqualityComparer<KeyValuePair<string, StatementSyntax>> EqualityComparer =
            new HashEqualityComparer<KeyValuePair<string, StatementSyntax>>(
                x =>
                    x.Key.GetHashCode());

        #endregion

        #region fields

        private readonly Dictionary<string, StatementSyntax> _statements;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CtorUpdater"/> class.
        /// </summary>
        /// <param name="ctorNode"></param>
        /// <param name="discoverer"></param>
        /// <param name="context"></param>
        public CtorUpdater(ConstructorDeclarationSyntax ctorNode, IDiscoverer discoverer, IRoslynContext context)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(ctorNode, nameof(ctorNode))
                .Parameter(discoverer, nameof(discoverer))
                .Parameter(context, nameof(context))
                .ThrowWhenNull();

            this._statements = ConstructPathDict(ctorNode, discoverer, context)
                .Distinct(EqualityComparer)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            this.UpdatedCtorNode = ctorNode.TrackNodes(this._statements.Values);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the updated node.
        /// </summary>
        public ConstructorDeclarationSyntax UpdatedCtorNode { get; private set; }

        #endregion

        #region members

        /// <summary>
        /// Remove all statements which start with the provided path.
        /// </summary>
        /// <param name="rootPath"></param>
        public void RemovePathWithChildren(string rootPath)
        {
            var statements = this._statements
                .Where(pair => pair.Key.StartsWith(rootPath))
                .Select(pair => this.UpdatedCtorNode.GetCurrentNode(pair.Value));

            this.UpdatedCtorNode = this.UpdatedCtorNode.RemoveNodes(statements, SyntaxRemoveOptions.KeepNoTrivia);
        }

        /// <summary>
        /// Update the constructor with the provided statements.
        /// Either add a new statement or update an existing one if the path is matching.
        /// </summary>
        /// <param name="configurationMemberEdits"></param>
        public void Update(IEnumerable<IConfigurationMemberEdit> configurationMemberEdits) =>
            configurationMemberEdits.ForEach(this.Update);

        private Maybe<StatementSyntax> FindStatement(string memberPath) =>
            this._statements.GetMaybe(memberPath)
                .Map(syntax => this.UpdatedCtorNode.GetCurrentNode(syntax));

        private void Update(IConfigurationMemberEdit configurationMemberEdit)
        {
            this.UpdatedCtorNode = this.FindStatement(configurationMemberEdit.MemberPath)
                .Map(node =>
                    configurationMemberEdit switch
                    {
                        AddConfigurationMemberEdit x => this.UpdatedCtorNode.ReplaceNode(node, x.Syntax),
                        _ => this.UpdatedCtorNode.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia),
                    })
                .SomeOrProvided(() =>
                    configurationMemberEdit switch
                    {
                        AddConfigurationMemberEdit x => this.UpdatedCtorNode.AddBodyStatements(x.Syntax),
                        _ => this.UpdatedCtorNode,
                    });
        }

        private static IEnumerable<KeyValuePair<string, StatementSyntax>> ConstructPathDict(
            ConstructorDeclarationSyntax ctorNode,
            IDiscoverer discoverer,
            IRoslynContext context)
        {
            if (ctorNode.Body == null)
            {
                return Enumerable.Empty<KeyValuePair<string, StatementSyntax>>();
            }

            var discoverFunc = discoverer.DiscoverMemberSelection;

            return ctorNode.Body.Statements
                .Select(
                    statement =>
                    {
                        var statementDescendant = statement
                            .DescendantNodes()
                            .Select(node => (node, context.Compilation.GetSemanticModel(node.SyntaxTree)));

                        return discoverFunc.Eval(statementDescendant)
                            .FirstOrNone()
                            .Map(
                                tuple => PathTreeBuilder.ConstructRootNode(
                                    context.Compilation.GetSemanticModel(tuple.IdentifierNameSyntax.SyntaxTree),
                                    tuple.IdentifierNameSyntax))
                            .Bind(root => root.DescendantNodes().LastOrNone())
                            .Map(pathNode => pathNode.ConstructPathToRoot(string.Empty).TrimStart('.'))
                            .Map(path => new KeyValuePair<string, StatementSyntax>(path, statement));
                    })
                .Somes();
        }
        #endregion
    }
}