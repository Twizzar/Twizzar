using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IResult{TSuccess,TFailure}"/>.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Cast a object to another type.
        /// </summary>
        /// <typeparam name="T">The type to cast to.</typeparam>
        /// <param name="o"></param>
        /// <returns>If the cast was successful return the casted object else a failure.</returns>
        public static IResult<T, Failure> CastTo<T>(this object o)
            where T : class =>
            Maybe.ToMaybe(o as T)
                .ToResult(new Failure($"Cannot cast {o.GetType().Name} to {typeof(T).Name}"));
    }
}