using System.Collections.Immutable;
using Moq;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.Factories;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.TestCommon.Builder
{
    /// <summary>
    /// Builder for the system default service behavior.
    /// </summary>
    public class SystemDefaultServiceBuilder
    {
        private ICtorSelector _ctorSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemDefaultServiceBuilder"/> class.
        /// </summary>
        public SystemDefaultServiceBuilder()
        {
            var ctorSelectorMock = new Mock<ICtorSelector>();

            ctorSelectorMock
                .Setup(selector => selector.GetCtorDescription(It.IsAny<ITypeDescription>(), CtorSelectionBehavior.Max))
                .Returns(() =>
                    Success(
                        Mock.Of<IMethodDescription>(description => 
                            description.Name == "ctor" &&
                            description.AccessModifier == AccessModifier.CreatePublic() &&
                            description.DeclaredParameters == ImmutableArray<IParameterDescription>.Empty &&
                            description.OverrideKind == OverrideKind.Create(false, false))));

            this._ctorSelector = ctorSelectorMock.Object;
        }

        public SystemDefaultServiceBuilder WithCtorSelector(ICtorSelector ctorSelector)
        {
            this._ctorSelector = ctorSelector;
            return this;
        }

        public ISystemDefaultService Build() =>
            new SystemDefaultService(this._ctorSelector, new ConfigurationItemFactory(
                null,
                (id, configurations, memberConfigurations, callbacks) =>
                        new ConfigurationItem(id, configurations, memberConfigurations, callbacks)));
    }
}
