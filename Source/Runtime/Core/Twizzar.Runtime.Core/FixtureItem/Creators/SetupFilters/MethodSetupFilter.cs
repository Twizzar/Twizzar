using System;
using System.Linq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators.Setup;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.SetupFilters
{
    /// <inheritdoc />
    public class MethodSetupFilter : IMethodSetupFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodSetupFilter"/> class.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="returnValue"></param>
        /// <param name="parameters"></param>
        public MethodSetupFilter(Maybe<string> methodName, object returnValue, Maybe<Type[]> parameters)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(methodName, nameof(methodName))
                .Parameter(parameters, nameof(parameters))
                .ThrowWhenNull();

            this.MethodName = methodName;
            this.ReturnValue = returnValue;
            this.Parameters = parameters;
        }

        #region Implementation of IMethodSetup

        /// <inheritdoc />
        public Maybe<string> MethodName { get; }

        /// <inheritdoc />
        public object ReturnValue { get; }

        /// <inheritdoc />
        public Maybe<Type[]> Parameters { get; }

        /// <inheritdoc />
        public bool IsMatching(IRuntimeMethodDescription methodDescription)
        {
            var parameterTypes = methodDescription.DeclaredParameters
                .Cast<IRuntimeParameterDescription>()
                .Select(description => description.Type);

            var nameIsMatching = this.MethodName
                .Map(s => s == methodDescription.Name)
                .SomeOrProvided(true);
            var parametersIsMatching = this.Parameters
                .Map(types => types.SequenceEqual(parameterTypes))
                .SomeOrProvided(true);

            var isAssignable = (this.ReturnValue is null && methodDescription.Type.CanBeNull())
                               || methodDescription.Type.IsInstanceOfType(this.ReturnValue);

            return nameIsMatching && parametersIsMatching && isAssignable;
        }

        #endregion
    }
}