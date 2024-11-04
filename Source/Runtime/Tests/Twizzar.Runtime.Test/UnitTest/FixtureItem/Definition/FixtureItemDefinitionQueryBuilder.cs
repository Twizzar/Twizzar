using System;
using System.Threading.Tasks;
using Moq;
using Twizzar.Runtime.Core.FixtureItem.Definition.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.Runtime.Test.UnitTest.FixtureItem.Builders;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

namespace Twizzar.Runtime.Test.UnitTest.FixtureItem.Definition
{
    public class FixtureItemDefinitionQueryBuilder
    {
        private readonly Mock<IConfigurationItemQuery> _configurationItemQueryMock;
        private readonly Mock<ITypeDescriptionQuery> _typeDescriptionQueryMock;
        private readonly IFixtureItemDefinitionNodeCreationService _creationService;

        public FixtureItemDefinitionQueryBuilder()
        {
            this._configurationItemQueryMock = new Mock<IConfigurationItemQuery>();
            this._typeDescriptionQueryMock = new Mock<ITypeDescriptionQuery>();
            this._creationService = new FixtureItemDefinitionNodeCreationServiceBuilder().Build();
        }

        public FixtureItemDefinitionQueryBuilder WithConfigurationItem(
            FixtureItemId id,
            IConfigurationItem configurationItem)
        {
            this._configurationItemQueryMock
                .Setup(query => query.GetConfigurationItem(id, It.IsAny<IRuntimeTypeDescription>()))
                .Returns(Task.FromResult(configurationItem));
            return this;
        }

        public FixtureItemDefinitionQueryBuilder WithTypeDescriptionNode(
            Type type,
            IRuntimeTypeDescription typeDescription)
        {
            this._typeDescriptionQueryMock.Setup(
                    query => query.GetTypeDescription(type))
                .Returns(typeDescription);
            this._typeDescriptionQueryMock.Setup(
                    query => query.GetTypeDescription(It.Is<ITypeFullName>(name => name.FullName == type.FullName)))
                .Returns(typeDescription);

            return this;
        }

        public FixtureItemDefinitionQuery Build() =>
            new FixtureItemDefinitionQuery(
                this._configurationItemQueryMock.Object,
                this._typeDescriptionQueryMock.Object,
                this._creationService);
    }
}
