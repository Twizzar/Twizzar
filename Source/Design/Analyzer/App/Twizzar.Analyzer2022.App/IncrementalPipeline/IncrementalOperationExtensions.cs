using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// Extension method for IIncrementalValuesOperation and IIncrementalValueOperation.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class IncrementalOperationExtensions
    {
        /// <summary>
        /// Convert to IncrementalOperation.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IIncrementalValuesOperation<TResult> ToIncrementalOperation<TResult>(this IValuesOperation<TResult> self) =>
            (IIncrementalValuesOperation<TResult>)self;

        /// <summary>
        /// Convert to IncrementalOperation.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IIncrementalValueOperation<TResult> ToIncrementalOperation<TResult>(this IValueOperation<TResult> self) =>
            (IIncrementalValueOperation<TResult>)self;

        /// <summary>
        /// Convert to <see cref="IncrementalValuesProvider{TValues}"/>.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IncrementalValuesProvider<TResult> ToIncrementalValuesProvider<TResult>(this IValuesOperation<TResult> self) =>
            self.ToIncrementalOperation().ValueProvider;

        /// <summary>
        /// Convert to <see cref="IncrementalValueProvider{TValue}"/>.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IncrementalValueProvider<TResult> ToIncrementalValueProvider<TResult>(this IValueOperation<TResult> self) =>
            self.ToIncrementalOperation().ValueProvider;
    }
}