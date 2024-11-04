using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.Validator;

partial class EnumValidatorTests
{
    private class EmptyIBaseDescriptionBuilder : ItemBuilder<IBaseDescription, EmptyIBaseDescriptionBuilderPaths>
    {
        public EmptyIBaseDescriptionBuilder()
        {
            this.With(p => p.IsEnum.Value(true));
        }
    }
}