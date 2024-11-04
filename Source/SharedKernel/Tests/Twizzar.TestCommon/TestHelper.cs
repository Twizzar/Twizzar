using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.TestCommon.Builder;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.TestCommon
{
    /// <summary>
    /// Helper class for unit tests.
    /// </summary>
    public static partial class TestHelper
    {
        #region static fields and constants

        private static readonly Random Rnd = new();

        #endregion

        #region members

        /// <summary>
        /// Generate a random string.
        /// </summary>
        /// <returns>A random string.</returns>
        public static string RandomString(string prefix = "") =>
            prefix + Guid.NewGuid().ToString();

        /// <summary>
        /// Generate a random char.
        /// </summary>
        /// <returns>A random char.</returns>
        public static char RandomChar() =>
            (char)RandomInt(0, 255);

        /// <summary>
        /// Generate a random int.
        /// </summary>
        /// <param name="start">Inclusive start.</param>
        /// <param name="end">Exclusive end.</param>
        /// <returns>A random int between start and end.</returns>
        public static int RandomInt(int start = int.MinValue, int end = int.MaxValue) =>
            Rnd.Next(start, end);

        /// <summary>
        /// Generate a random double.
        /// </summary>
        /// <param name="start">Inclusive start.</param>
        /// <param name="end">Exclusive end.</param>
        /// <returns>A random double between start and end.</returns>
        public static double RandomDouble(double start = double.MinValue, double end = double.MaxValue)
        {
            var nextDouble = Rnd.NextDouble();
            return (end * nextDouble) + (start * (1d - nextDouble));
        }

        /// <summary>
        /// Generate a random boolean.
        /// </summary>
        /// <returns>A random boolean.</returns>
        public static bool RandomBool() =>
            Rnd.NextDouble() > 0.5;

        /// <summary>
        /// Generate a random numeric with suffix
        /// </summary>
        /// <returns>A random numeric with suffix</returns>
        public static NumericWithSuffix GetRandomNumericWithSuffix(Maybe<char> suffix)
        {
            var number = RandomDouble();
            return new NumericWithSuffix($"{number}", suffix);
        }

        /// <summary>
        /// Create a random nameless fixture id.
        /// </summary>
        /// <returns>The fixture id.</returns>
        public static FixtureItemId RandomNamelessFixtureItemId() =>
            FixtureItemId.CreateNameless(RandomTypeFullName());

        /// <summary>
        /// Create a random named fixture id.
        /// </summary>
        /// <returns>The fixture id.</returns>
        public static FixtureItemId RandomNamedFixtureItemId(
            string prefixRootItemPath = "",
            string prefixName = "",
            string prefixType = "") =>
            FixtureItemId.CreateNamed(
                prefixName + RandomString(),
                RandomTypeFullName(prefixType))
            .WithRootItemPath(prefixRootItemPath + RandomString());

        /// <summary>
        /// Create a random Type Full Name.
        /// </summary>
        /// <param name="prefix">A prefix for the random type name.</param>
        /// <param name="namespaceSegments">How many namespace segments should be generated.
        /// 0 for generating a global TypeName, -1 for choosing a random number between 0 and 4.</param>
        /// <returns>A type full name.</returns>
        public static ITypeFullName RandomTypeFullName(string prefix = "", int namespaceSegments = -1) =>
            new TypeFullNameBuilder(prefix + CreateRandomFullName(namespaceSegments)).Build();

        private static string CreateRandomFullName(int namespaceParts)
        {
            EnsureHelper.GetDefault.Parameter(namespaceParts, nameof(namespaceParts))
                .IsGreaterEqualThan(-1)
                .ThrowOnFailure();

            static string CreateNamePart() =>
                "T" +
                RandomString('a', 'z', RandomInt(1,10))
                    .TakeWhile(c => c != '-')
                    .AsString();

            var repeatCount = namespaceParts == -1 ? RandomInt(1, 5) : namespaceParts + 1;

            return string.Join(".", Repeat(CreateNamePart, repeatCount));
        }

        private static string RandomString(char start, char end, int length) =>
            string.Concat(
                Enumerable.Range(0, length)
                    .Select(_ => (char)Rnd.Next(start, end)));

        /// <summary>
        /// Assert that the result should be successful and on success return the success value.
        /// </summary>
        /// <typeparam name="TSuccess">Success type.</typeparam>
        /// <typeparam name="TFailure">Failure type.</typeparam>
        /// <param name="result">The result monad.</param>
        /// <returns>The success value.</returns>
        public static TSuccess AssertAndUnwrapSuccess<TSuccess, TFailure>(IResult<TSuccess, TFailure> result)
            where TFailure : Failure
        {
            result.IsSuccess.Should().BeTrue(result.ToString());
            return result.GetSuccessUnsafe();
        }

        public static IEnumerable<T> Repeat<T>(Func<T> creator, int count) =>
            Enumerable.Repeat(0, count).Select(_ => creator());

        #endregion
    }
}