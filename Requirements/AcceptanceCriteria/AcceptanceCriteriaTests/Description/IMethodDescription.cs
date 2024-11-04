using System.Collections.Immutable;

namespace AcceptanceCriteriaTests.Description
{
    /// <summary>
    /// Provides information about methods and constructors.
    /// </summary>
    internal interface IMethodDescription : IMemberDescription
    {
        #region description

        /// <summary>
        /// Gets the parameters as a string.
        /// </summary>
        string Parameters { get; }

        /// <summary>
        /// Gets the declared parameter as a list.
        /// </summary>
        ImmutableArray<IParameterDescription> DeclaredParameters { get; }

        /// <summary>
        /// Gets the parameter full types as a string.
        /// </summary>
        string FriendlyParameterFullTypes { get; }

        /// <summary>
        /// Gets a value indicating whether the method is a constructor.
        /// </summary>
        bool IsConstructor { get; }

        /// <summary>
        /// Gets a value indicating whether the method is abstract.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Gets a value indicating whether the method is static.
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        /// Gets a value indicating whether the methods has generic parameter.
        /// </summary>
        bool IsGeneric { get; }

        /// <summary>
        /// Gets the method kind.
        /// </summary>
        MethodKind MethodKind { get; }

        #endregion
    }

    public enum MethodKind
    {
        Method,
        Property,
        Void
    }
}