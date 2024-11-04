using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Shell;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using Task = System.Threading.Tasks.Task;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.ConfigWriter
{
    /// <summary>
    /// Context for the <see cref="RoslynConfigWriter"/>.
    /// </summary>
    public class Context : RoslynContext
    {
        #region fields

        private readonly ClassDeclarationSyntax _oldNode;
        private readonly HashSet<string> _usingSet;
        private readonly HashSet<string> _newUsingSet = new();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="builderInformation"></param>
        public Context(
            IRoslynContext context,
            IBuilderInformation builderInformation)
            : base(context)
        {
            this._oldNode = builderInformation.ClassDeclarationSyntax;
            this.CurrentConfigNode = builderInformation.ClassDeclarationSyntax;

            this._usingSet = this.CurrentConfigNode.SyntaxTree.GetRoot()
                .DescendantNodesAndSelf()
                .OfType<UsingDirectiveSyntax>()
                .Select(syntax => syntax.Name.ToString())
                .ToHashSet();
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the current config node.
        /// </summary>
        public ClassDeclarationSyntax CurrentConfigNode { get; set; }

        #endregion

        #region members

        /// <summary>
        /// Update the using statements.
        /// </summary>
        /// <param name="namespace"></param>
        public void UpdateUsings(string @namespace)
        {
            if (!this._usingSet.Contains(@namespace))
            {
                this._usingSet.Add(@namespace);
                this._newUsingSet.Add(@namespace);
            }
        }

        /// <summary>
        /// Sync the changes to the workspace.
        /// </summary>
        /// <param name="workspace"></param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        public async Task SyncAsync(Workspace workspace)
        {
            var root = await this._oldNode.SyntaxTree.GetRootAsync();

            var newRoot = root.ReplaceNode(
                    this._oldNode,
                    this.CurrentConfigNode)
                .NormalizeWhitespace();

            if (this._newUsingSet.Count > 0)
            {
                var compilationUnit = newRoot.DescendantNodesAndSelf()
                    .OfType<CompilationUnitSyntax>()
                    .Single();

                var newCompilationUnit = this._newUsingSet
                    .Aggregate(
                        compilationUnit,
                        (current, ns) => current.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(ns))));

                newRoot = await newRoot.ReplaceNode(compilationUnit, newCompilationUnit)
                    .NormalizeWhitespace()
                    .SyntaxTree.GetRootAsync();
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            workspace.ReplaceNode(root, newRoot)
                .IfFailure(failure => this.Log(failure.Message, LogLevel.Error));
        }

        #endregion
    }
}