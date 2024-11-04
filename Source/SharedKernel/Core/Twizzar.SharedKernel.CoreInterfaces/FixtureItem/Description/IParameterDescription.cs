using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Gets the attributes for this parameter.
    /// </summary>
    public interface IParameterDescription : IBaseDescription, IValueObject
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the parameters default value.
        /// </summary>
        Maybe<ParameterDefaultValue> DefaultValue { get; }

        /// <summary>
        /// Gets a value indicating whether this is an input parameter.
        /// </summary>
        bool IsIn { get; }

        /// <summary>
        /// Gets a value indicating whether this parameter is optional.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// Gets a value indicating whether this is an out parameter.
        /// </summary>
        bool IsOut { get; }

        /// <summary>
        /// Gets the parameter position in the method.
        /// </summary>
        int Position { get; }
    }
}