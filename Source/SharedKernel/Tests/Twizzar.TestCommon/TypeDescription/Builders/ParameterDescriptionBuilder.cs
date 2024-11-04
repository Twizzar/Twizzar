using System;
using System.Collections.Immutable;
using Moq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

namespace Twizzar.TestCommon.TypeDescription.Builders
{
    public class ParameterDescriptionBuilder
    {
        public string Name { get; private set; }

        public ITypeFullName Type { get; private set; }

        public bool IsBaseType { get; private set; }

        public ITypeDescription ReturnTypeDescription { get; private set; }

        public ParameterDescriptionBuilder()
        {
            this.Name = Guid.NewGuid().ToString();
            this.Type = TestHelper.RandomTypeFullName();
            this.ReturnTypeDescription = null;
        }

        public ParameterDescriptionBuilder WithName(string name)
        {
            this.Name = name;
            return this;
        }

        public ParameterDescriptionBuilder WithType(ITypeFullName type)
        {
            this.Type = type;
            return this;
        }

        public ParameterDescriptionBuilder WithBaseType(bool isBaseType)
        {
            this.IsBaseType = isBaseType;
            return this;
        }

        public ParameterDescriptionBuilder WithReturnTypeDescription(ITypeDescription typeDescription)
        {
            this.ReturnTypeDescription = typeDescription;
            return this;
        }

        public IParameterDescription Build()
        {
            var mock = new Mock<IParameterDescription>();
            mock.Setup(description => description.Name).Returns(this.Name);
            mock.Setup(description => description.TypeFullName).Returns(this.Type);
            mock.Setup(description => description.IsBaseType).Returns(this.IsBaseType);
            mock.Setup(description => description.ArrayDimension).Returns(ImmutableArray<int>.Empty);
            mock.Setup(description => description.GetFriendlyReturnTypeFullName())
                .Returns(this.ReturnTypeDescription?.TypeFullName?.GetFriendlyCSharpTypeFullName());

            this.ReturnTypeDescription ??= new TypeDescriptionBuilder().WithTypeFullName(this.Type)
                .Build();

            mock.Setup(description => description.GetReturnTypeDescription())
                .Returns(() => this.ReturnTypeDescription);

            return mock.Object;
        }
    }
}
