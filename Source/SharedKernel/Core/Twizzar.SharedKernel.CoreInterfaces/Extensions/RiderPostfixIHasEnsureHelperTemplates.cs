using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using ViCommon.EnsureHelper.ArgumentHelpers;

// ReSharper disable InconsistentNaming

#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable SA1005 // Single line comments should begin with single space
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Resharper Live Templates for the IHasEnsureHelper extenstion methods.
    /// see: https://www.jetbrains.com/help/rider/Source_Templates.html#how-it-works.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class RiderPostfixIHasEnsureHelperTemplates
    {
        [SourceTemplate]

        // ReSharper disable once InconsistentNaming
        public static void param<T>(
            this ParameterValidatorCollection<T> source,
            [Macro(Expression = "completeSmart()")] string parameter)
        {
            /*$ $source$
                .Parameter($parameter$, nameof($parameter$))$END$ */
        }

        [SourceTemplate]
        public static void ensure<T>(
            this T source)
        {
            //$this.EnsureParameter(source, nameof(source))$END$
        }

        [SourceTemplate]
        public static void ensureNotNull<T>(
            this T source)
            where T : class
        {
            //$this.EnsureParameter(source, nameof(source)).ThrowWhenNull();$END$
        }

        [SourceTemplate]
        public static void ensureMany<T>(
            this T source,
            [Macro(Expression = "guessExpectedElementType()")] string paramType,
            [Macro(Expression = "completeSmart()")] string parameter)
            where T : class
        {
            /*$ this.EnsureMany<$paramType$>()
                .Parameter($source$, nameof($source$))
                .Parameter($parameter$, nameof($parameter$))$END$ */
        }

        [SourceTemplate]
        public static void ensureManyVariousTypes<T>(
            this T source,
            [Macro(Expression = "completeSmart()")] string parameter)
            where T : class
        {
            /*$ this.EnsureMany()
                .Parameter($source$, nameof($source$))
                .Parameter($parameter$, nameof($parameter$))$END$ */
        }

        [SourceTemplate]
        public static void ensureCtorParameterNotNull<T>(
            this T source)
            where T : class
        {
            /*$this.EnsureCtorParameterIsNotNull($source$, nameof($source$));$END$ */
        }
    }
}
