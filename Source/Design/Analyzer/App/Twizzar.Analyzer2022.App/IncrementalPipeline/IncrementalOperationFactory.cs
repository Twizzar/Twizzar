using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// Factory for creating operation for the Incremental Generator.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IncrementalOperationFactory : IOperationFactory
    {
        #region fields

        private readonly IncrementalGeneratorInitializationContext _context;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementalOperationFactory"/> class.
        /// </summary>
        /// <param name="context"></param>
        public IncrementalOperationFactory(IncrementalGeneratorInitializationContext context)
        {
            this._context = context;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public IValuesOperation<TSource> Init<TSource>(
            Func<SyntaxNode, CancellationToken, bool> predicate,
            Func<(SyntaxNode Node, SemanticModel SemanticModel), CancellationToken, TSource> transform) =>
            new InitOperation<TSource>(
                this._context.CompilationProvider
                    .Select((compilation, _) =>
                        compilation.WithOptions(compilation.Options.WithMetadataImportOptions(MetadataImportOptions.All)))
                    .SelectMany((compilation, _) =>
                        compilation.SyntaxTrees.Select(tree => (tree, compilation: compilation.GetSemanticModel(tree, true))))
                    .SelectMany((t, token) =>
                        t.tree.GetRoot(token).DescendantNodesAndSelf().Select(node => (node, t.compilation)))
                    .Where(t =>
                        predicate(t.node, CancellationToken.None))
                    .Select((t, token) =>
                        transform(t, token)),
                this);

        /// <inheritdoc />
        public IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> Init() =>
            new InitOperation<(SyntaxNode Node, SemanticModel SemanticModel)>(
                this._context.SyntaxProvider.CreateSyntaxProvider(
                    (_, _) => true,
                    (context, _) => (context.Node, context.SemanticModel)),
                this);

        /// <inheritdoc />
        public IValueOperation<Compilation> GetCompilation() =>
            new IncrementalValueOperation<Compilation>(this._context.CompilationProvider, this);

        /// <inheritdoc />
        public IValuesOperation<TSource> Where<TSource>(
            IValuesOperation<TSource> prev,
            Func<TSource, bool> predicate) =>
            new WhereOperation<TSource>(predicate, prev.ToIncrementalOperation());

        /// <inheritdoc />
        public IValuesOperation<TResult> Select<TSource, TResult>(
            IValuesOperation<TSource> prev,
            Func<TSource, CancellationToken, TResult> selector) =>
            new SelectValuesOperation<TSource, TResult>(selector, prev.ToIncrementalOperation());

        /// <inheritdoc />
        public IValueOperation<TResult> Select<TSource, TResult>(
            IValueOperation<TSource> prev,
            Func<TSource, CancellationToken, TResult> selector) =>
            new SelectValueOperation<TSource, TResult>(selector, prev.ToIncrementalOperation());

        /// <inheritdoc />
        public IValuesOperation<TResult> SelectMany<TSource, TResult>(
            IValuesOperation<TSource> prev,
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector) =>
            throw new NotImplementedException();

        /// <inheritdoc />
        public IValuesOperation<TResult> SelectMany<TSource, TResult>(
            IValueOperation<TSource> prev,
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector) =>
            new SelectManyOperation<TSource, TResult>(selector, prev.ToIncrementalOperation());

        /// <inheritdoc />
        public IValueOperation<(TLeft Left, TRight Right)> Combine<TLeft, TRight>(
            IValueOperation<TLeft> operation1,
            IValueOperation<TRight> operation2) =>
            new CombineOperation<TLeft, TRight>(
                operation1.ToIncrementalOperation(),
                operation2.ToIncrementalOperation());

        /// <inheritdoc />
        public IValuesOperation<(TLeft Left, TRight Right)> Combine<TLeft, TRight>(
            IValuesOperation<TLeft> operation1,
            IValueOperation<TRight> operation2) =>
            new BatchOperation<TLeft, TRight>(
                operation1.ToIncrementalOperation(),
                operation2.ToIncrementalOperation());

        /// <inheritdoc />
        public IValueOperation<ImmutableArray<TSource>> Collect<TSource>(IValuesOperation<TSource> source) =>
            new CollectOperation<TSource>(source.ToIncrementalOperation());

        #endregion
    }
}