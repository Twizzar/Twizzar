using Twizzar.Fixture;
using DemoCode.ExampleCode;

namespace AcceptanceCriteriaTests
{
    partial class PropertiesTests
    {
        private class PropertiesTest320bBuilder : ItemBuilder<DemoCode.ExampleCode.PropertiesTest, PropertiesTest320bBuilderPaths>
        {
            public PropertiesTest320bBuilder()
            {
                this.With(p => p.PropertyWithBackingField.Value(5));
            }
        }
    }
}