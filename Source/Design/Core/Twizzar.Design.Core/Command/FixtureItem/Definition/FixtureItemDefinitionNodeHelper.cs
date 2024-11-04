using Twizzar.Design.CoreInterfaces.Command.Events;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Core.Command.FixtureItem.Definition
{
    /// <summary>
    /// Static helper methods for <see cref="FixtureItemDefinitionNode"/>.
    /// </summary>
    public static class FixtureItemDefinitionNodeHelper
    {
        /// <summary>
        /// Wrap an event into a failure result.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <param name="e">The event to wrap.</param>
        /// <returns>A failure result.</returns>
        public static IResult<TSuccess, FailedEventFailure<FixtureItemCreatedFailedEvent>> Failure<TSuccess>(FixtureItemCreatedFailedEvent e) =>
            Result.Failure<TSuccess, FailedEventFailure<FixtureItemCreatedFailedEvent>>(
                new FailedEventFailure<FixtureItemCreatedFailedEvent>(e));

        /// <summary>
        /// Wrap a event into a failure result.
        /// </summary>
        /// <param name="e">The event to wrap.</param>
        /// <returns>A failure result.</returns>
        public static IResult<Maybe<FixtureItemCreatedEvent>, FailedEventFailure<FixtureItemMemberChangedFailedEvent>>
            Failure(FixtureItemMemberChangedFailedEvent e) =>
                Result
                    .Failure<Maybe<FixtureItemCreatedEvent>,
                        FailedEventFailure<FixtureItemMemberChangedFailedEvent>>(
                            new FailedEventFailure<FixtureItemMemberChangedFailedEvent>(e));

        /// <summary>
        /// Wrap a event into a success result.
        /// </summary>
        /// <param name="e">The event to wrap.</param>
        /// <returns>A success result.</returns>
        public static IResult<FixtureItemCreatedEvent, FailedEventFailure<FixtureItemCreatedFailedEvent>> Success(
            FixtureItemCreatedEvent e) =>
                Result.Success<FixtureItemCreatedEvent, FailedEventFailure<FixtureItemCreatedFailedEvent>>(e);
    }
}