using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.Resources;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.SharedKernel.Core.FixtureItem.Configuration.Services
{
    /// <summary>
    /// Implements the <see cref="ICtorSelector"/> interfaces and is responsible to determine the ctor for given types.
    /// </summary>
    public class CtorSelector : ICtorSelector
    {
        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of ICtorSelector

        /// <inheritdoc />
        public Result<IMethodDescription, InvalidTypeDescriptionFailure> GetCtorDescription(
            ITypeDescription typeDescription,
            CtorSelectionBehavior ctorSelectionBehavior)
        {
            this.EnsureParameter(typeDescription, nameof(typeDescription))
                .IsNotNull()
                .ThrowOnFailure();

            if (!(typeDescription.IsClass || typeDescription.IsStruct))
            {
                return new InvalidTypeDescriptionFailure(
                    typeDescription,
                    ErrorMessages.CtorSelector_InvalidConfiguration);
            }

            var ctorDescriptions = GetValidCtors(typeDescription).ToArray();
            if (ctorDescriptions.Length == 0)
            {
                return new InvalidTypeDescriptionFailure(
                    typeDescription,
                    ErrorMessages.CtorSelector_ClassWithoutCtor);
            }

            return ctorSelectionBehavior switch
            {
                CtorSelectionBehavior.Max => DetermineMaxCtor(typeDescription, ctorDescriptions),
                CtorSelectionBehavior.Min => DetermineMinCtor(typeDescription, ctorDescriptions),
                _ => throw new NotImplementedException(),
            };
        }

        /// <inheritdoc />
        public Maybe<IMethodDescription> FindCtor(
            IConfigurationItem config,
            ITypeDescription typeDescription)
        {
            this.EnsureParameter(config, nameof(config)).ThrowWhenNull();
            this.EnsureParameter(typeDescription, nameof(typeDescription)).ThrowWhenNull();

            var ctorConfig = config.CtorConfiguration;

            return ctorConfig.IsSome && ctorConfig.GetValueUnsafe() is CtorMemberConfiguration ctorMemberConfig
                ? GetValidCtors(typeDescription)
                    .FirstOrNone(ctor =>
                        ctor.DeclaredParameters.Select(p => p.TypeFullName)
                            .SequenceEqual(ctorMemberConfig.ConstructorSignature))
                : None();
        }

        #endregion

        private static int CalculateHashCode(ImmutableArray<IParameterDescription> values)
        {
            var concatValues = new StringBuilder();
            foreach (var parameterDescription in values)
            {
                concatValues.Append(parameterDescription.TypeFullName.FullName + parameterDescription.Position);
            }

            return concatValues.ToString().GetHashCode();
        }

        private static IEnumerable<IMethodDescription> GetValidCtors(ITypeDescription typeDescription)
        {
            // Fist find all public ctors
            var publicCtors = typeDescription.GetDeclaredConstructors()
                .Where(ctor => ctor.AccessModifier.IsPublic)
                .ToArray();

            // When there are non return all Non public.
            return publicCtors.Any()
                ? publicCtors
                : typeDescription.GetDeclaredConstructors()
                    .Where(description => !description.AccessModifier.IsPublic);
        }

        private static Result<IMethodDescription, InvalidTypeDescriptionFailure> DetermineMaxCtor(
            ITypeDescription typeDescription, IMethodDescription[] ctorDescriptions) =>
            DetermineCtor(typeDescription, GetMaxCtorParamLength(ctorDescriptions));

        private static Result<IMethodDescription, InvalidTypeDescriptionFailure> DetermineMinCtor(
            ITypeDescription typeDescription, IMethodDescription[] ctorDescriptions) =>
            DetermineCtor(typeDescription, GetMinCtorParamLength(ctorDescriptions));

        private static Result<IMethodDescription, InvalidTypeDescriptionFailure> DetermineCtor(ITypeDescription typeDescription, int ctorParams)
        {
            var filteredCtors =
                FilterDeclaredConstructors(typeDescription, ctorParams);

            if (!filteredCtors.Any())
            {
                return new InvalidTypeDescriptionFailure(typeDescription, ErrorMessages.CtorSelector_ClassWithoutCtor);
            }

            if (filteredCtors.Length == 1)
            {
                return Success(filteredCtors.First());
            }

            var selectCtor = filteredCtors
                .Aggregate(
                    (agg, next)
                        => CalculateHashCode(next.DeclaredParameters) > CalculateHashCode(agg.DeclaredParameters)
                            ? next
                            : agg);

            return Success(selectCtor);
        }

        private static ImmutableArray<IMethodDescription> FilterDeclaredConstructors(ITypeDescription typeDescription, int ctorParamLength) =>
            typeDescription.GetDeclaredConstructors()
                .Where(ctor => ctor.DeclaredParameters.Length == ctorParamLength)
                .ToImmutableArray();

        private static int GetMaxCtorParamLength(IMethodDescription[] ctorDescriptions) =>
            ctorDescriptions
                .Max(ctor => ctor.DeclaredParameters.Length);

        private static int GetMinCtorParamLength(IMethodDescription[] ctorDescriptions) =>
            ctorDescriptions
                .Min(ctor => ctor.DeclaredParameters.Length);
    }
}