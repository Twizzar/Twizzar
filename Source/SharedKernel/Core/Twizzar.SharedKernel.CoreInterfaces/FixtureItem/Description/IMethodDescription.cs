using System.Collections.Immutable;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Provides information about methods and constructors.
    /// </summary>
    public interface IMethodDescription : IMemberDescription
    {
        #region description

        /// <summary>
        /// Gets the name + method parameter to make the method unique.
        /// </summary>
        string UniqueName { get; }

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
        /// Gets the generic type arguments.
        /// </summary>
        ImmutableDictionary<int, GenericParameterType> GenericTypeArguments { get; }

        /// <summary>
        /// Gets the override kind opf the method.
        /// </summary>
        OverrideKind OverrideKind { get; }

        /// <summary>
        /// Gets the method kind.
        /// </summary>
        MethodKind MethodKind { get; }

        #endregion
    }
}