using System;

using Moq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

namespace Twizzar.TestCommon.TypeDescription.Builders
{
    public class PropertyDescriptionBuilder
    {
        public ITypeFullName Type { get; private set; }

        public string Name { get; private set; }
        public bool IsBaseType { get; private set; }

        public bool IsNullableBaseType { get; private set; }

        private bool _canRead;
        private bool _canWrite;
        private ITypeDescription _returnTypeDescription;

        public PropertyDescriptionBuilder()
        {
            this.Name = Guid.NewGuid().ToString();
            this.Type = TestHelper.RandomTypeFullName();
        }

        public PropertyDescriptionBuilder WithName(string name)
        {
            this.Name = name;
            return this;
        }

        public PropertyDescriptionBuilder WithType(ITypeFullName type)
        {
            this.Type = type;
            return this;
        }

        public PropertyDescriptionBuilder WithBaseType(bool isBaseType)
        {
            this.IsBaseType = isBaseType;
            return this;
        }

        public PropertyDescriptionBuilder WithNullableBaseType(bool isBaseType)
        {
            this.IsNullableBaseType = isBaseType;
            return this;
        }

        public PropertyDescriptionBuilder WithReturnType(ITypeDescription typeDescription)
        {
            this._returnTypeDescription = typeDescription;
            return this;
        }

        public PropertyDescriptionBuilder WithCanRead(bool canRead)
        {
            this._canRead = canRead;
            return this;
        }

        public PropertyDescriptionBuilder WithCanWrite(bool canWrite)
        {
            this._canWrite = canWrite;
            return this;
        }

        public IPropertyDescription Build(Func<Mock<IPropertyDescription>, Mock<IPropertyDescription>> additionalSetup = null) =>
            this.Build<IPropertyDescription>(additionalSetup);

        public T Build<T>(Func<Mock<T>, Mock<T>> additionalSetup = null)
            where T : class, IPropertyDescription
        {
            var mock = new Mock<T>();

            if (additionalSetup != null)
            {
                mock = additionalSetup(mock);
            }

            mock.Setup(description => description.Name)
                .Returns(this.Name);
            mock.Setup(description => description.TypeFullName)
                .Returns(this.Type);
            mock.Setup(description => description.AccessModifier)
                .Returns(AccessModifier.CreatePublic);
            mock.Setup(desc => desc.IsBaseType)
                .Returns(this.IsBaseType);
            mock.Setup(desc => desc.IsNullableBaseType)
                .Returns(this.IsNullableBaseType);
            mock.Setup(description => description.GetReturnTypeDescription())
                .Returns(this._returnTypeDescription);
            mock.Setup(description => description.GetFriendlyReturnTypeFullName())
                .Returns(this._returnTypeDescription?.TypeFullName?.GetFriendlyCSharpTypeFullName());
            mock.Setup(description => description.CanWrite)
                .Returns(this._canWrite);
            mock.Setup(description => description.CanRead)
                .Returns(this._canRead);
            return mock.Object;
        }
    }
}
