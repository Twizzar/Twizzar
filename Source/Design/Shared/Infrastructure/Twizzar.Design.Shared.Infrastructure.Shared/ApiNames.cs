using System.Collections.Immutable;

namespace Twizzar.Design.Shared.Infrastructure
{
    /// <summary>
    /// Names like types form the Twizzar Api.
    /// </summary>
    public static class ApiNames
    {
        /// <summary>
        /// Gets the api namespace.
        /// </summary>
        public const string ApiNamespace = "Twizzar.Fixture";

        /// <summary>
        /// Gets the path provider base class name.
        /// </summary>
        public const string PathProviderBaseClassName = "PathProvider`1";

        /// <summary>
        /// Gets the ItemBuilder name with two generic type arguments.
        /// </summary>
        public const string ItemBuilderT2Name = "ItemBuilder`2";

        /// <summary>
        /// Gets the IItemBuilder name with two generic type arguments.
        /// </summary>
        public const string IItemBuilderT2Name = "IItemBuilder`2";

        /// <summary>
        /// Gets the ItemBuilder with one generic type argument.
        /// </summary>
        public const string ItemBuilderT1Name = "ItemBuilder`1";

        /// <summary>
        /// Gets the IPathSelectionProvider name with one generic type argument.
        /// </summary>
        public const string IPathSelectionProviderT1Name = "IPathSelectionProvider`1";

        /// <summary>
        /// Gets the IPathSelectionProvider name with two generic type argument.
        /// </summary>
        public const string IPathSelectionProviderT2Name = "IPathSelectionProvider`2";

        /// <summary>
        /// Gets the name of the ItemBuilderBase class.
        /// </summary>
        public const string ItemBuilderBase = "ItemBuilderBase`1";

        /// <summary>
        /// Gets the name of the ItemBuilder without the generic postfix.
        /// </summary>
        public const string ItemBuilderName = "ItemBuilder";

        /// <summary>
        /// Gets the postfix used for new default path providers.
        /// </summary>
        public const string PathProviderPostfix = "Path";

        /// <summary>
        /// The project name of the analyzer without a postfix like 2022 for the vs version.
        /// </summary>
        public const string AnalyzerProjectNamePrefix = "Twizzar.Analyzer";

        /// <summary>
        /// The namespace for the ProjectStatistics class.
        /// </summary>
        public const string ProjectStatisticsNameSpace = "Twizzar";

        /// <summary>
        /// The name of the ProjectStatistics class.
        /// </summary>
        public const string ProjectStatisticsTypeName = "ProjectStatistics";

        /// <summary>
        /// Gets the type full name of the ProjectStatistics class.
        /// </summary>
        public const string ProjectStatisticsTypeFullName = $"{ProjectStatisticsNameSpace}.{ProjectStatisticsTypeName}";

        /// <summary>
        /// Gets the name of the BuilderInvocationCount field.
        /// </summary>
        public const string BuilderInvocationCountMemberName = "BuilderInvocationCount";

        /// <summary>
        /// Get all reserved members of the PathProvider classes.
        /// </summary>
        public static readonly ImmutableHashSet<string> ReservedPathProviderMembers =
            ImmutableHashSet.Create(
                "TzParent",
                "ViDeclaredType",
                "ViName",
                "ViUniqueName",
                "ViMemberName",
                "ViReturnType",
                "ViParent",
                "ToString",
                "Equals",
                "GetHashCode",
                "GetMethodPostfix",
                "GetParameterString",
                "Value",
                "Unique",
                "Undefined",
                "InstanceOf",
                "Stub",
                "Ctor");
    }
}
