using System;
using System.Collections.Immutable;
using System.Linq;
using Moq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Creators.SetupFilters
{
    partial class MethodSetupFilterTests
    {
        private class StringIRuntimeMethodDescriptionBuilder : ItemBuilder<Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.IRuntimeMethodDescription, IRuntimeMethodDescription86c7BuilderPaths>
        {
            public StringIRuntimeMethodDescriptionBuilder()
            {
                this.With(p => p.Type.Value(typeof(string)));
            }
        }

        private class MethodDescriptionParameterBuilder : ItemBuilder<Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.IRuntimeMethodDescription, IRuntimeMethodDescriptiona944BuilderPaths>
        {
            public MethodDescriptionParameterBuilder WithParameterTypes(params Type[] parameterTypes)
            {
                IRuntimeParameterDescription ToDescription(Type t) =>
                    Mock.Of<IRuntimeParameterDescription>(description => description.Type == t);

                var declaredParameter = parameterTypes.Select(ToDescription)
                    .Cast<IParameterDescription>()
                    .ToImmutableArray();

                this.With(p => p.DeclaredParameters.Value(declaredParameter));
                this.With(p => p.Type.Value(typeof(string)));
                return this;
            }
        }
    }
}