using DemoCode.Interfaces;
using Twizzar.Fixture;

namespace DemoCode.Tests
{
    public partial class EmailMessageBufferTests
    {

        private class AlwaysValidValidationServiceBuilder : ItemBuilder<IValidationService, AlwaysValidValidationServicePaths>
        {
            public AlwaysValidValidationServiceBuilder()
            {
                this.With(p => p.Validate.Value(true));
            }
        }

        private class DefaultEmailMessageBufferBuilder : ItemBuilder<EmailMessageBuffer, DefaultEmailMessageBufferPaths>
        {
            public DefaultEmailMessageBufferBuilder()
            {
                this.With(p => p.Ctor.validationService.Validate.Value(true));
            }
        }
    }
}