using Moq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.Test.UnitTest.FixtureItem.Builders
{
    public class RuntimePropertyDescriptionBuilder
    {
        private readonly Mock<IRuntimePropertyDescription> _mock;


        public RuntimePropertyDescriptionBuilder()
        {
            this._mock = new Mock<IRuntimePropertyDescription>();
        }

        public RuntimePropertyDescriptionBuilder WithName(string name)
        {
            this._mock.Setup(description => description.Name)
                .Returns(name);

            return this;
        }

        public IRuntimePropertyDescription Build() =>
            this._mock.Object;
    }
}
