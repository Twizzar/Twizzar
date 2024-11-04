using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using Moq;
using Twizzar.Runtime.CoreInterfaces.Helpers;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Fixture
{
    /// <summary>
    /// Exception throw when a verification failed.
    /// </summary>
    [Serializable]
    public class VerificationException : Exception
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public VerificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationException"/> class.
        ///
        /// Supports the serialization infrastructure.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected VerificationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region members

        /// <summary>
        /// Supports the serialization infrastructure.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Construct a <see cref="VerificationException"/>.
        /// </summary>
        /// <param name="mockException">The inner mock exception.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="count">The count asked in the verify.</param>
        /// <param name="mock">The mock which was verified.</param>
        /// <param name="mockType">The mock inner type.</param>
        /// <param name="whereInformation">Information describing the selected member.</param>
        /// <param name="predicate">Predicate for selecting invocation als valid or invalid for this verification.</param>
        /// <returns></returns>
        internal static VerificationException Construct(
            MockException mockException,
            MethodInfo methodInfo,
            Maybe<int> count,
            object mock,
            Type mockType,
            string whereInformation,
            Predicate<IInvocation> predicate)
        {
            var sb = new StringBuilder();

            var countMessage = count.AsMaybeValue() switch
            {
                SomeValue<int> { Value: 1 } => $"exactly one invocation",
                SomeValue<int> v => $"exactly {v.Value} invocations",
                _ => $"at least one invocation",
            };

            sb.Append($"Expected {countMessage} on {mockType.Name}, but ");

            try
            {
                var invocations = GetInvocations(mock, mockType).ToList();

                var matches = invocations
                    .Where(invocation => invocation.Method.Equals(methodInfo))
                    .Where(invocation => predicate(invocation))
                    .ToList();

                sb.Append(
                    matches.Count switch
                    {
                        0 => "no invocation was performed.",
                        1 => "one invocation was performed.",
                        var x => $"{x} invocations where performed.",
                    });

                sb.AppendLine().AppendLine("Where:");
                sb.AppendLine(whereInformation);

                if (invocations.Count > 0)
                {
                    sb.AppendLine().AppendLine("Performed invocations:");

                    foreach (var invocation in invocations)
                    {
                        sb.AppendLine($"    {invocation}");
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return new VerificationException(sb.ToString(), mockException);
        }

        private static IEnumerable<IInvocation> GetInvocations(object mock, Type mockType) =>
            ReflectionGenericMethodBuilder.Create(
                    new Func<Mock<object>, IEnumerable<IInvocation>>(GetInvocations))
                .WithGenericTypes(mockType)
                .WithParameters(mock)
                .Invoke<IEnumerable<IInvocation>>();

        private static IEnumerable<IInvocation> GetInvocations<TMockObject>(Mock<TMockObject> mock)
            where TMockObject : class =>
            mock.Invocations;

        #endregion
    }
}