using Moq;
using Twizzar.CompositionRoot.Factory;
using Twizzar.Runtime.Core.FixtureItem.Definition;
using Twizzar.Runtime.Core.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.Core.FixtureItem.Definition.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;

namespace Twizzar.Runtime.Test.UnitTest.FixtureItem.Builders
{
    public class FixtureItemDefinitionNodeCreationServiceBuilder
    {
        private readonly FixtureDefinitionFactory.BaseTypeFactory _baseTypeFactory = 
            (description, id, definition, nullable) => new BaseTypeNode(description, id, definition, nullable);

        private readonly FixtureDefinitionFactory.ClassFactory _classFactory =
            (description, id, type, properties, parameters, methods, fields, creatorType) =>
                new ClassNode(description, id, type, properties, parameters, methods, fields, creatorType);

        private readonly FixtureDefinitionFactory.MockFactory _mockFactory =
            (description, id, type, properties, methods, creatorType) => 
                new MockNode(description, id, type, properties, methods, creatorType);

        private readonly FixtureDefinitionFactory.ParameterFactory _parameterFactory =
            (configuration, description) => new ParameterDefinition(configuration, description);

        private readonly FixtureDefinitionFactory.PropertyFactory _propertyFactory =
            (configuration, description) => new PropertyDefinition(configuration, description);

        private readonly FixtureDefinitionFactory.MethodFactory _methodFactory =
            (configuration, description, callbacks) => new MethodDefinition(configuration, description, callbacks);

        private readonly FixtureDefinitionFactory.FieldFactory _fieldFactory =
            (configuration, description) => new FieldDefinition(configuration, description);

        public Mock<ICtorSelector> CtorSelectorMock = new Mock<ICtorSelector>();

        public FixtureItemDefinitionNodeCreationService Build() =>
            new(
                new FixtureDefinitionFactory(
                        null,
                        this._baseTypeFactory, 
                        this._classFactory, 
                        this._mockFactory, 
                        this._propertyFactory,
                        this._fieldFactory,
                        this._parameterFactory,
                        this._methodFactory),
                this.CtorSelectorMock.Object);
    }
}
