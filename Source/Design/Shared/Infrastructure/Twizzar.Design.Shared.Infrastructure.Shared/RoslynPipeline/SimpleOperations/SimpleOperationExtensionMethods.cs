using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// Extension methods for <see cref="ISimpleValueOperation{TSource}"/> and <see cref="ISimpleValuesOperation{TSource}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SimpleOperationExtensionMethods
    {
        /// <summary>
        /// Convert to a simple operation.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="self"></param>
        /// <returns>.</returns>
        public static ISimpleValuesOperation<TSource> ToSimpleOperation<TSource>(this IValuesOperation<TSource> self) =>
            (ISimpleValuesOperation<TSource>)self;

        /// <summary>
        /// Convert to a simple operation.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="self"></param>
        /// <returns>.</returns>
        public static ISimpleValueOperation<TSource> ToSimpleOperation<TSource>(this IValueOperation<TSource> self) =>
            (ISimpleValueOperation<TSource>)self;
    }
}