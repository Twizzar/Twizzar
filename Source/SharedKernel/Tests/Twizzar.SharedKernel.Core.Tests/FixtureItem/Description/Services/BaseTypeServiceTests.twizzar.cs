using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;

namespace Twizzar.SharedKernel.Core.Tests.FixtureItem.Description.Services
{
    partial class BaseTypeServiceTests
    {
        private class EnumIBaseDescriptionBuilder : ItemBuilder<Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.IBaseDescription, IBaseDescription0143BuilderPaths>
        {
            public EnumIBaseDescriptionBuilder()
            {
                this.With(p => p.GetReturnTypeDescription.Stub<ITypeDescription>());
                this.With(p => p.GetReturnTypeDescription.IsEnum.Value(true));
            }
        }
    }
}