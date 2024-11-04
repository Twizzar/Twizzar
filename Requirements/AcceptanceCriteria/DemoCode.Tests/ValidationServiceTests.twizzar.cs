using DemoCode.Interfaces;
using Twizzar.Fixture;

namespace DemoCode.Tests
{
    public partial class ValidationServiceTests
    {
        private class ValidEmailMessageBuilder : ItemBuilder<EmailMessage, ValidEmailMessagePaths>
        {
            public ValidEmailMessageBuilder()
            {
                this.With(p => p.Header.Value("This is a valid email header"));
            }
        }
    }
}