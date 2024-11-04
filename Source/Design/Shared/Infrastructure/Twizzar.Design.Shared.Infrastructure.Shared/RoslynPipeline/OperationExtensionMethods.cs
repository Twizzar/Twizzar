using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline
{
    /// <summary>
    /// Extension method for using the <see cref="IValuesOperation{TSource}"/> and <see cref="IValuesOperation{TSource}"/>.
    /// </summary>
    public static class OperationExtensionMethods
    {
        #region 1 => 1 transform

        /// <summary>
        /// <code>
        /// <![CDATA[
        ///                                    Select<TSource, TResult>
        ///                                    .......................................
        ///                                    .                   ┌───────────┐     .
        ///                                    .   selector(Item1) │           │     .
        ///                                    . ┌────────────────►│  Result1  ├───┐ .
        ///                                    . │                 │           │   │ .
        /// IValuesOperation<TSource>          . │                 └───────────┘   │ . IValuesOperation<TResult>
        ///           ┌───────────┐            . │                 ┌───────────┐   │ .        ┌────────────┐
        ///           │           │            . │ selector(Item2) │           │   │ .        │            │
        ///           │  TSource  ├──────────────┼────────────────►│  Result2  ├───┼─────────►│   TResult  │
        ///           │           │            . │                 │           │   │ .        │            │
        ///           └───────────┘            . │                 └───────────┘   │ .        └────────────┘
        ///             3 items                . │                 ┌───────────┐   │ .            3 items
        ///      [Item1, Item2, Item3]         . │ selector(Item3) │           │   │ .  [Result1, Result2, Result3]
        ///                                    . └────────────────►│  Result3  ├───┘ .
        ///                                    .                   │           │     .
        ///                                    .                   └───────────┘     .
        ///                                    .......................................
        /// ]]>
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="self"></param>
        /// <param name="selector"></param>
        /// <returns>A new <see cref="IValuesOperation{TSource}"/>.</returns>
        public static IValuesOperation<TResult> Select<TSource, TResult>(
            this IValuesOperation<TSource> self,
            Func<TSource, CancellationToken, TResult> selector) =>
            self.OperationFactory.Select(self, selector);

        /// <summary>
        /// <code>
        /// <![CDATA[
        ///  IValueOperation<TSource>                       IValueOperation<TResult>
        /// ┌─────────────┐                                 ┌─────────────┐
        /// │             │    Select<TSource,TResult>      │             │
        /// │   TSource   ├───────────────────────────────► │   TResult   │
        /// │             │                                 │             │
        /// └─────────────┘                                 └─────────────┘
        /// ]]>
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="self"></param>
        /// <param name="selector"></param>
        /// <returns>A new <see cref="IValueOperation{TSource}"/>.</returns>
        public static IValueOperation<TResult> Select<TSource, TResult>(
            this IValueOperation<TSource> self,
            Func<TSource, CancellationToken, TResult> selector) =>
            self.OperationFactory.Select(self, selector);

        #endregion

        #region 1 => many (or none) transform

        /// <summary>
        /// Allows transformation form 1 to many. <see cref="SelectMany{TSource,TResult}(IValuesOperation{TSource},System.Func{TSource,System.Threading.CancellationToken,System.Collections.Immutable.ImmutableArray{TResult}})"/> for more information.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="self"></param>
        /// <param name="selector"></param>
        /// <returns>A new <see cref="IValuesOperation{TSource}"/>.</returns>
        public static IValuesOperation<TResult> SelectMany<TSource, TResult>(
            this IValueOperation<TSource> self,
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector) =>
            self.OperationFactory.SelectMany(self, selector);

        /// <summary>
        /// Allows transformation form 1 to many. <see cref="SelectMany{TSource,TResult}(IValuesOperation{TSource},System.Func{TSource,System.Threading.CancellationToken,System.Collections.Immutable.ImmutableArray{TResult}})"/> for more information.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="self"></param>
        /// <param name="selector"></param>
        /// <returns>A new <see cref="IValuesOperation{TSource}"/>.</returns>
        public static IValuesOperation<TResult> SelectMany<TSource, TResult>(
            this IValueOperation<TSource> self,
            Func<TSource, CancellationToken, IEnumerable<TResult>> selector) =>
            self.OperationFactory.SelectMany(self, (source, token) => selector(source, token).ToImmutableArray());

        /// <summary>
        /// Allows transformation of many to many items.
        /// <code>
        /// <![CDATA[
        ///                                        SelectMany<TSource, TResult>
        ///                              ...............................................
        ///                              .                        ┌─────────┐          .
        ///                              .                        │         │          .
        ///                              .                  ┌────►│ Result1 ├───────┐  .
        ///                              .                  │     │         │       │  .
        ///                              .                  │     └─────────┘       │  .
        ///                              .  selector(Item1) │                       │  .
        ///                              .┌─────────────────┘     ┌─────────┐       │  .
        ///                              .│                       │         │       │  .
        ///     IValuesOperation<TSource>.│                 ┌────►│ Result2 ├───────┤  .    IValuesOperation<TResult>
        ///     ┌───────────┐            .│                 │     │         │       │  .            ┌────────────┐
        ///     │           │            .│ selector(Item2) │     └─────────┘       │  .            │            │
        ///     │  TSource  ├─────────────┼─────────────────┤     ┌─────────┐       ├──────────────►│  TResult   │
        ///     │           │            .│                 │     │         │       │  .            │            │
        ///     └───────────┘            .│                 └────►│ Result3 ├───────┤  .            └────────────┘
        ///        3 items               .│                       │         │       │  .               7 items
        ///  [Item1, Item2, Item3]       .│ selector(Item3)       └─────────┘       │  .  [Result1, Result2, Result3, Result4,
        ///                              .└─────────────────┐                       │  .      Result5, Result6, Result7 ]
        ///                              .                  │     ┌─────────┐       │  .
        ///                              .                  │     │         │       │  .
        ///                              .                  ├────►│ Result4 ├───────┤  .
        ///                              .                  │     │         │       │  .
        ///                              .                  │     └─────────┘       │  .
        ///                              .                  │     ┌─────────┐       │  .
        ///                              .                  │     │         │       │  .
        ///                              .                  ├────►│ Result5 ├───────┤  .
        ///                              .                  │     │         │       │  .
        ///                              .                  │     └─────────┘       │  .
        ///                              .                  │     ┌─────────┐       │  .
        ///                              .                  │     │         │       │  .
        ///                              .                  └────►│ Result6 ├───────┘  .
        ///                              .                        │         │          .
        ///                              .                        └─────────┘          .
        ///                              ...............................................
        /// ]]>
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="self"></param>
        /// <param name="selector"></param>
        /// <returns>A new <see cref="IValuesOperation{TSource}"/>.</returns>
        public static IValuesOperation<TResult> SelectMany<TSource, TResult>(
            this IValuesOperation<TSource> self,
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector) =>
            self.OperationFactory.SelectMany(self, selector);

        /// <summary>
        /// Allows transformation of many to many items.
        /// <code>
        /// <![CDATA[
        ///                                        SelectMany<TSource, TResult>
        ///                              ...............................................
        ///                              .                        ┌─────────┐          .
        ///                              .                        │         │          .
        ///                              .                  ┌────►│ Result1 ├───────┐  .
        ///                              .                  │     │         │       │  .
        ///                              .                  │     └─────────┘       │  .
        ///                              .  selector(Item1) │                       │  .
        ///                              .┌─────────────────┘     ┌─────────┐       │  .
        ///                              .│                       │         │       │  .
        ///     IValuesOperation<TSource>.│                 ┌────►│ Result2 ├───────┤  .    IValuesOperation<TResult>
        ///     ┌───────────┐            .│                 │     │         │       │  .            ┌────────────┐
        ///     │           │            .│ selector(Item2) │     └─────────┘       │  .            │            │
        ///     │  TSource  ├─────────────┼─────────────────┤     ┌─────────┐       ├──────────────►│  TResult   │
        ///     │           │            .│                 │     │         │       │  .            │            │
        ///     └───────────┘            .│                 └────►│ Result3 ├───────┤  .            └────────────┘
        ///        3 items               .│                       │         │       │  .               7 items
        ///  [Item1, Item2, Item3]       .│ selector(Item3)       └─────────┘       │  .  [Result1, Result2, Result3, Result4,
        ///                              .└─────────────────┐                       │  .      Result5, Result6, Result7 ]
        ///                              .                  │     ┌─────────┐       │  .
        ///                              .                  │     │         │       │  .
        ///                              .                  ├────►│ Result4 ├───────┤  .
        ///                              .                  │     │         │       │  .
        ///                              .                  │     └─────────┘       │  .
        ///                              .                  │     ┌─────────┐       │  .
        ///                              .                  │     │         │       │  .
        ///                              .                  ├────►│ Result5 ├───────┤  .
        ///                              .                  │     │         │       │  .
        ///                              .                  │     └─────────┘       │  .
        ///                              .                  │     ┌─────────┐       │  .
        ///                              .                  │     │         │       │  .
        ///                              .                  └────►│ Result6 ├───────┘  .
        ///                              .                        │         │          .
        ///                              .                        └─────────┘          .
        ///                              ...............................................
        /// ]]>
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="self"></param>
        /// <param name="selector"></param>
        /// <returns>A new <see cref="IValuesOperation{TSource}"/>.</returns>
        public static IValuesOperation<TResult> SelectMany<TSource, TResult>(
            this IValuesOperation<TSource> self,
            Func<TSource, CancellationToken, IEnumerable<TResult>> selector) =>
            self.OperationFactory.SelectMany(self, (source, token) => selector(source, token).ToImmutableArray());

        /// <summary>
        /// Collect values into an <see cref="ImmutableArray{T}"/>.
        /// <code>
        /// <![CDATA[
        /// IValuesOperation<TSource>                    IValueOperation<ImmutableArray<TSource>>
        ///      ┌───────────┐                                  ┌─────────────────────────┐
        ///      │           │          Collect<TSource>        │                         │
        ///      │  TSource  ├─────────────────────────────────►│ ImmutableArray<TSource> │
        ///      │           │                                  │                         │
        ///      └───────────┘                                  └─────────────────────────┘
        ///         3 Items                                             Single Item
        ///
        ///          Item1                                         [Item1, Item2, Item3]
        ///          Item2
        ///          Item3
        /// ]]>
        /// </code>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns>A new <see cref="IValueOperation{TSource}"/>.</returns>
        public static IValueOperation<ImmutableArray<TSource>>
            Collect<TSource>(this IValuesOperation<TSource> source) =>
            source.OperationFactory.Collect(source);

        /// <summary>
        /// Combine tow values to a tuple.
        /// <code>
        /// <![CDATA[
        /// IValueOperation<TSource1>
        /// ┌───────────┐
        /// │           │
        /// │  TSource1 ├────────────────┐
        /// │           │                │                                 IValueOperation<(TSource1, TSource2)>
        /// └───────────┘                │
        ///  Single Item                 │                                          ┌────────────────────────┐
        ///                              │       Combine<TSource1, TSource2>        │                        │
        ///    Item1                     ├─────────────────────────────────────────►│  (TSource1, TSource2)  │
        ///                              │                                          │                        │
        /// IValueOperation<TSource2>    │                                          └────────────────────────┘
        /// ┌───────────┐                │                                                   Single Item
        /// │           │                │
        /// │  TSource2 ├────────────────┘                                                  (Item1, Item2)
        /// │           │
        /// └───────────┘
        ///  Single Item
        ///
        ///    Item2
        /// ]]>
        /// </code>
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="provider1"></param>
        /// <param name="provider2"></param>
        /// <returns>A new <see cref="IValueOperation{TSource}"/>.</returns>
        public static IValueOperation<(TLeft Left, TRight Right)> Combine<TLeft, TRight>(
            this IValueOperation<TLeft> provider1,
            IValueOperation<TRight> provider2) =>
            provider1.OperationFactory.Combine(provider1, provider2);

        /// <summary>
        /// Combine values with a value.
        /// <code>
        /// <![CDATA[
        /// IValuesOperation<TSource1>
        ///  ┌───────────┐
        ///  │           │
        ///  │  TSource1 ├────────────────┐
        ///  │           │                │
        ///  └───────────┘                │
        ///     3 Items                   │                                IValuesOperation<(TSource1, TSource2)>
        ///                               │
        ///    LeftItem1                  │                                          ┌────────────────────────┐
        ///    LeftItem2                  │       Combine<TSource1, TSource2>        │                        │
        ///    LeftItem3                  ├─────────────────────────────────────────►│  (TSource1, TSource2)  │
        ///                               │                                          │                        │
        ///                               │                                          └────────────────────────┘
        ///  IValueOperation<TSource2>    │                                                  3 Items
        ///  ┌───────────┐                │
        ///  │           │                │                                            (LeftItem1, RightItem)
        ///  │  TSource2 ├────────────────┘                                            (LeftItem2, RightItem)
        ///  │           │                                                             (LeftItem3, RightItem)
        ///  └───────────┘
        ///   Single Item
        ///
        ///    RightItem
        /// ]]>
        /// </code>
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="provider1"></param>
        /// <param name="provider2"></param>
        /// <returns>A new <see cref="IValuesOperation{TSource}"/>.</returns>
        public static IValuesOperation<(TLeft Left, TRight Right)> Combine<TLeft, TRight>(
            this IValuesOperation<TLeft> provider1,
            IValueOperation<TRight> provider2) =>
            provider1.OperationFactory.Combine(provider1, provider2);

        #endregion

        #region helper for filtering

        /// <summary>
        /// Filtering the items.
        /// <code>
        /// <![CDATA[
        ///                                       Where<TSource>
        ///                           .......................................
        ///                           .                   ┌───────────┐     .
        ///                           .   predicate(Item1)│           │     .
        ///                           . ┌────────────────►│   Item1   ├───┐ .
        ///                           . │                 │           │   │ .
        /// IValuesOperation<TSource> . │                 └───────────┘   │ .     IValuesOperation<TResult>
        ///  ┌───────────┐            . │                                 │ .        ┌────────────┐
        ///  │           │            . │ predicate(Item2)                │ .        │            │
        ///  │  TSource  ├──────────────┼─────────────────X               ├─────────►│   TResult  │
        ///  │           │            . │                                 │ .        │            │
        ///  └───────────┘            . │                                 │ .        └────────────┘
        ///     3 Items               . │                 ┌───────────┐   │ .           2 Items
        ///                           . │ predicate(Item3)│           │   │ .
        ///                           . └────────────────►│   Item3   ├───┘ .
        ///                           .                   │           │     .
        ///                           .                   └───────────┘     .
        ///                           .......................................
        /// ]]>
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="self"></param>
        /// <param name="predicate"></param>
        /// <returns>A new <see cref="IValuesOperation{TSource}"/>.</returns>
        public static IValuesOperation<TSource> Where<TSource>(
            this IValuesOperation<TSource> self,
            Func<TSource, bool> predicate) =>
            self.OperationFactory.Where(self, predicate);

        #endregion

        #region Helper methods

        /// <summary>
        /// Filter by type and convert to TType.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <param name="self"></param>
        /// <returns>A new <see cref="IValuesOperation{TSource}"/>.</returns>
        public static IValuesOperation<TType> OfType<TSource, TType>(this IValuesOperation<TSource> self)
            where TType : TSource =>
            self.Where(source => source is TType)
                .Select((source, token) => (TType)source);

        /// <summary>
        /// Filter out all Nones.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="self"></param>
        /// <returns>A new <see cref="IValuesOperation{TSource}"/>.</returns>
        public static IValuesOperation<TSource> Somes<TSource>(this IValuesOperation<Maybe<TSource>> self) =>
            self.Where(source => source.IsSome)
                .Select((maybe, token) => maybe.GetValueUnsafe());

        /// <summary>
        /// Evalualte an function which useses <see cref="IValuesOperation{TSource}"/>.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="self"></param>
        /// <param name="input"></param>
        /// <returns>The evaluated values.</returns>
        public static IEnumerable<TOutput> Eval<TInput, TOutput>(this Func<IValuesOperation<TInput>, IValuesOperation<TOutput>> self, IEnumerable<TInput> input)
        {
            var factory = new SimpleOperationFactory(CancellationToken.None);

            return self(new WrapValuesOperation<TInput>(input, factory, CancellationToken.None))
                .ToSimpleOperation()
                .Evaluate();
        }

        #endregion
    }
}