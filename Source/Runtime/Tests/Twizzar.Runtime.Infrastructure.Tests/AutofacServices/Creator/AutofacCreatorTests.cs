using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Fixture;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.Infrastructure.AutofacServices.Creator;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon;

namespace Twizzar.Runtime.Infrastructure.Tests.AutofacServices.Creator
{
    [TestFixture]
    public class AutofacCreatorTests
    {
        private Mock<IInstanceCacheRegistrant> _instanceCacheRegistrantMock;
        private Mock<IBaseTypeCreator> _baseTypeCreatorMock;
        private AutofacCreatorTestImplementation _sut;

        [SetUp]
        public void SetUp()
        {
            this._instanceCacheRegistrantMock = new Mock<IInstanceCacheRegistrant>();
            this._baseTypeCreatorMock = new Mock<IBaseTypeCreator>();

            this._sut = new AutofacCreatorTestImplementation(
                this._instanceCacheRegistrantMock.Object,
                this._baseTypeCreatorMock.Object);
        }

        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<AutofacCreatorTestImplementation>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void Resolved_instances_gets_registered_in_the_cache_service()
        {
            // arrange
            var memberName = "TestField";
            var id = TestHelper.RandomNamedFixtureItemId();
            var idName = id.Name.SomeOrProvided(() => "");

            var valueDefinition = Mock.Of<IUniqueDefinition>();
            var description = Mock.Of<IFieldDescription>(fieldDescription => fieldDescription.Name == memberName);

            // act
            this._sut.ResolveType(valueDefinition, description, id);

            // assert
            this._instanceCacheRegistrantMock.Verify(
                registrant => registrant.Register($"{idName}.{memberName}", null),
                Times.Once);
        }

        [Test]
        public void Resolved_instances_gets_not_registered_in_the_cache_service_when_id_name_is_none()
        {
            // arrange
            var memberName = "TestField";
            var id = TestHelper.RandomNamelessFixtureItemId();

            var valueDefinition = Mock.Of<IUniqueDefinition>();
            var description = Mock.Of<IFieldDescription>(fieldDescription => fieldDescription.Name == memberName);

            // act
            this._sut.ResolveType(valueDefinition, description, id);

            // assert
            this._instanceCacheRegistrantMock.Verify(
                registrant => registrant.Register(It.IsAny<string>(), It.IsAny<object>()),
                Times.Never);
        }

        [Test]
        public void NullValue_will_be_resolved_as_null()
        {
            // arrange
            var memberName = "TestField";
            var id = TestHelper.RandomNamelessFixtureItemId();


            var valueDefinition = Mock.Of<IUniqueDefinition>();
            var description = Mock.Of<IFieldDescription>(fieldDescription => fieldDescription.Name == memberName);

            this._baseTypeCreatorMock
                .Setup(creator => creator.CreateInstance(valueDefinition, description))
                .Returns(Mock.Of<INullValue>());

            // act
            var value = this._sut.ResolveType(valueDefinition, description, id);

            // assert
            value.Should().BeNull();
        }

        public class AutofacCreatorTestImplementation : AutofacCreator
        {
            /// <inheritdoc />
            public AutofacCreatorTestImplementation(IInstanceCacheRegistrant instanceCacheRegistrant, IBaseTypeCreator baseTypeCreator)
                : base(instanceCacheRegistrant, baseTypeCreator)
            {
            }

            #region Overrides of AutofacCreator

            /// <inheritdoc />
            public override object CreateInstance(IFixtureItemDefinitionNode definition) =>
                throw new System.NotImplementedException();

            public new object ResolveType(
                IValueDefinition definition,
                IBaseDescription description,
                FixtureItemId parentId) =>
                    base.ResolveType(definition, description, parentId);

            #endregion
        }
    }
}