using System;
using Twizzar.CompositionRoot.Factory;
using Twizzar.Runtime.Core.FixtureItem.Description;
using Twizzar.SharedKernel.Core.FixtureItem.Description.Services;

namespace Twizzar.Runtime.Test.Builder
{
    public class ReflectionDescriptionFactoryBuilder
    {
        internal ReflectionDescriptionFactory Build() => new(null, Factory);

        private static ReflectionTypeDescription Factory(Type type) => 
            new(type, new BaseTypeService());
    }
}