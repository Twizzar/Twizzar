using System;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extension Methods for <see cref="IBaseDescription"/>.
    /// </summary>
    public static class BaseDescriptionExtension
    {
        #region members

        /// <summary>
        /// Get the member path.
        /// </summary>
        /// <param name="baseDescription">Should of type
        /// <see cref="IFieldDescription"/>,
        /// <see cref="IMethodDescription"/>,
        /// <see cref="IPropertyDescription"/>
        /// or <see cref="IParameterDescription"/>.
        /// </param>
        /// <returns>The member path.</returns>
        /// <exception cref="ArgumentOutOfRangeException">When the baseDescription is not of the correct type.</exception>
        public static string GetMemberPathName(this IBaseDescription baseDescription)
        {
            var path = baseDescription switch
            {
                IMethodDescription methodDescription =>
                    methodDescription.IsConstructor
                        ? "Ctor"
                        : methodDescription.UniqueName,
                IMemberDescription memberDescription => memberDescription.Name,
                IParameterDescription parameter => parameter.Name,
                ITypeDescription _ => string.Empty,
                _ => throw new PatternErrorBuilder(nameof(baseDescription))
                    .IsNotOneOf(
                        nameof(IMethodDescription),
                        nameof(IMemberDescription),
                        nameof(IParameterDescription),
                        nameof(ITypeDescription)),
            };

            return path?.ToSourceVariableCodeFriendly();
        }

        #endregion
    }
}