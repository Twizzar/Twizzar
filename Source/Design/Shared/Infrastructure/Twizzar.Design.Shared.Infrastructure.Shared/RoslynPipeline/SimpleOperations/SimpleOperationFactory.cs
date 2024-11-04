using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <inheritdoc cref="IOperationFactory" />
    [ExcludeFromCodeCoverage]
    public class SimpleOperationFactory : IOperationFactory
    {
        #region fields

        private readonly SyntaxNode _root;
        private readonly SemanticModel _semanticModel;
        private readonly CancellationToken _cancellationToken;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleOperationFactory"/> class.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="semanticModel"></param>
        /// <param name="cancellationToken"></param>
        public SimpleOperationFactory(SyntaxNode root, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(root, nameof(root))
                .Parameter(semanticModel, nameof(semanticModel))
                .Parameter(cancellationToken, nameof(cancellationToken))
                .ThrowWhenNull();

            this._root = root;
            this._semanticModel = semanticModel;
            this._cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleOperationFactory"/> class.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public SimpleOperationFactory(CancellationToken cancellationToken)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(cancellationToken, nameof(cancellationToken))
                .ThrowWhenNull();

            this._cancellationToken = cancellationToken;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public IValueOperation<ImmutableArray<TSource>> Collect<TSource>(IValuesOperation<TSource> source) =>
            new CollectOperation<TSource>(source.ToSimpleOperation());

        /// <inheritdoc />
        public IValueOperation<(TLeft Left, TRight Right)> Combine<TLeft, TRight>(
            IValueOperation<TLeft> operation1,
            IValueOperation<TRight> operation2) =>
            new CombineOperation<TLeft, TRight>(operation1.ToSimpleOperation(), operation2.ToSimpleOperation());

        /// <inheritdoc />
        public IValuesOperation<(TLeft Left, TRight Right)> Combine<TLeft, TRight>(
            IValuesOperation<TLeft> operation1,
            IValueOperation<TRight> operation2) =>
            new BatchOperation<TLeft, TRight>(operation1.ToSimpleOperation(), operation2.ToSimpleOperation());

        /// <inheritdoc />
        public IValuesOperation<TSource> Init<TSource>(
            Func<SyntaxNode, CancellationToken, bool> predicate,
            Func<(SyntaxNode Node, SemanticModel SemanticModel), CancellationToken, TSource> transform) =>
            new InitOperation<TSource>(
                this._root,
                this._semanticModel,
                predicate,
                transform,
                this,
                this._cancellationToken);

        /// <inheritdoc />
        public IValueOperation<Compilation> GetCompilation() => throw new NotImplementedException();

        /// <inheritdoc />
        public IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> Init() =>
            new InitOperation<(SyntaxNode Node, SemanticModel SemanticModel)>(
                this._root,
                this._semanticModel,
                (_, _) => true,
                (tuple, _) => tuple,
                this,
                this._cancellationToken);

        /// <inheritdoc />
        public IValuesOperation<TResult> Select<TSource, TResult>(
            IValuesOperation<TSource> prev,
            Func<TSource, CancellationToken, TResult> selector) =>
            new SelectValuesOperation<TSource, TResult>(selector, prev.ToSimpleOperation());

        /// <inheritdoc />
        public IValueOperation<TResult> Select<TSource, TResult>(
            IValueOperation<TSource> prev,
            Func<TSource, CancellationToken, TResult> selector) =>
            new SelectValueOperation<TSource, TResult>(selector, prev.ToSimpleOperation());

        /// <inheritdoc />
        public IValuesOperation<TResult> SelectMany<TSource, TResult>(
            IValuesOperation<TSource> prev,
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector) =>
            new SelectManyOperation<TSource, TResult>(selector, prev.ToSimpleOperation());

        /// <inheritdoc />
        public IValuesOperation<TResult> SelectMany<TSource, TResult>(
            IValueOperation<TSource> prev,
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector) =>
            new SelectManyOperation<TSource, TResult>(selector, prev.ToSimpleOperation());

        /// <inheritdoc />
        public IValuesOperation<TSource> Where<TSource>(
            IValuesOperation<TSource> prev,
            Func<TSource, bool> predicate) =>
            new WhereOperation<TSource>(predicate, prev.ToSimpleOperation());

        #endregion
    }
}