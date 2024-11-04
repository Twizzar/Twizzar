using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using ViCommon.EnsureHelper.ArgumentHelpers;

#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Resharper Live Templates for the ensure helper.
    /// see: https://www.jetbrains.com/help/rider/Source_Templates.html#how-it-works.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class RiderPostfixEnsureHelperTemplates
    {
        [SourceTemplate]
        public static void param<T>(
            this ParameterValidatorCollection<T> source,
            [Macro(Expression = "completeSmart()")] string parameter)
        {
            /*$ $source$
                .Parameter($parameter$, nameof($parameter$))$END$ */
        }
    }
}
