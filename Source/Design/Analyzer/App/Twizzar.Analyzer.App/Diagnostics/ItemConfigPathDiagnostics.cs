using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Twizzar.Analyzer.Core.Diagnostics
{
    /// <summary>
    /// DiagnosticDescriptor for ItemConfigPathSourceGenerator.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ItemConfigPathDiagnostics
    {
        #region static fields and constants

        /// <summary>
        /// Warning diagnostic when an unexpected error occurred.
        /// </summary>
        public static readonly DiagnosticDescriptor UnexpectedDiagnosticDescriptor = new(
            id: $"{IdPrefix}0001",
            title: "Unexpected error occurred in Path Generation",
            messageFormat: "{0}",
            category: Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: new Uri("https://github.com/Twizzar/Twizzar/wiki/Analyzer-diagnostics#tz0001").AbsoluteUri);

        /// <summary>
        /// Warning diagnostic when an fixture item which is private or protected is requested.
        /// </summary>
        public static readonly DiagnosticDescriptor OnlyPublicInternalSupported = new(
            id: $"{IdPrefix}0002",
            title: "Only public and internal types are supported.",
            messageFormat: "{0}",
            category: Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: new Uri("https://github.com/Twizzar/Twizzar/wiki/Analyzer-diagnostics#tz0002").AbsoluteUri);

        private const string Category = "twizzar";
        private const string IdPrefix = "TZ";

        #endregion
    }
}