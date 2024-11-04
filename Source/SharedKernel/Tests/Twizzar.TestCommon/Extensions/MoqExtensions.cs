using System;
using System.Linq.Expressions;
using Moq;
using Moq.Language;

namespace Twizzar.TestCommon.Extensions
{
    /// <summary>
    /// Extension class for Moq.
    /// </summary>
    public static class MoqExtensions
    {
        /// <summary>
        /// Check if the object is a mocked object.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="instance">The object instance.</param>
        /// <returns>True if it is mocked else false.</returns>
        public static bool IsAMock<T>(this T instance)
            where T : class
        {
            try
            {
                Mock.Get(instance);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Combine the <see cref="Mock{T}.Setup"/> and <see cref="IReturns{TMock,TResult}.Returns(TResult)"/> and returns the mock.
        /// </summary>
        /// <typeparam name="T">The mock type.</typeparam>
        /// <typeparam name="TResult">The result of the member to configure.</typeparam>
        /// <param name="mock">The mock.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="returns">The configured return type.</param>
        /// <returns>The mock.</returns>
        public static Mock<T> Setup<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression, TResult returns)
            where T : class
        {
            mock.Setup(expression)
                .Returns(returns);

            return mock;
        }
    }
}
