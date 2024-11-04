using System;
using System.Diagnostics.CodeAnalysis;
using ViCommon.EnsureHelper.ArgumentHelpers;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Merge with <see cref="ParameterValidationStringExtenstion"/>.
    /// </summary>
    public static class EnsureParameterValidationStringExtenstion
    {
        /// <summary>
        /// Checks if the string is empty.
        /// </summary>
        /// <param name="parameterValidator">The argument validator.</param>
        /// <param name="customExceptionBuilder">Function which has as input the parameter name and builds a custom <see cref="Exception"/> to throw on failure.</param>
        /// <returns>A new <see cref="IParameterValidator{TParam}"/> to chain  more checks or react to the validation.</returns>
        [ExcludeFromCodeCoverage]
        public static IParameterValidator<string> IsNotNullAndNotEmpty(
            this IParameterValidator<string> parameterValidator,
            Func<string, Exception> customExceptionBuilder) =>
            parameterValidator.IsNotNull(customExceptionBuilder).IsNotEmpty(customExceptionBuilder);
    }
}
