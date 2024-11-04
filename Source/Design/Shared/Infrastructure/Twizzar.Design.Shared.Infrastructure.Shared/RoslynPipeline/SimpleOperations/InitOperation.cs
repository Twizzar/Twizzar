using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// An initial operation for chaining other operations.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="Root"></param>
    /// <param name="SemanticModel"></param>
    /// <param name="Predicate"></param>
    /// <param name="Transform"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="CancellationToken"></param>
    [ExcludeFromCodeCoverage]
    public record InitOperation<TSource>(
        SyntaxNode Root,
        SemanticModel SemanticModel,
        Func<SyntaxNode, CancellationToken, bool> Predicate,
        Func<(SyntaxNode Node, SemanticModel SemanticModel), CancellationToken, TSource> Transform,
        IOperationFactory OperationFactory,
        CancellationToken CancellationToken)
        : SimpleValuesOperation<TSource>(OperationFactory, CancellationToken)
    {
        /// <inheritdoc />
        public override IEnumerable<TSource> Evaluate()
        {
            this.CancellationToken.ThrowIfCancellationRequested();

            return this.Root.DescendantNodesAndSelf()
                .Where(node => this.Predicate(node, this.CancellationToken))
                .Select(node => this.Transform((node, this.SemanticModel), this.CancellationToken));
        }
    }
}