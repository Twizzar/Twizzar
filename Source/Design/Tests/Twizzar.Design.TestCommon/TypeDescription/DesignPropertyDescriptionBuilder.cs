using System;
using Moq;
using Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.TestCommon.TypeDescription
{
    public class DesignPropertyDescriptionBuilder
    {
        public string Type { get; private set; }
        public string Name { get; private set; }
        public bool IsBaseType { get; private set; }
        public bool IsInterface { get; private set; }
        public bool CanWrite { get; private set; }
        public bool IsAutoProperty { get; private set; }

        public DesignPropertyDescriptionBuilder()
        {
            this.Name = Guid.NewGuid().ToString();
            this.Type = Guid.NewGuid().ToString();
        }

        public DesignPropertyDescriptionBuilder WithName(string name)
        {
            this.Name = name;
            return this;
        }

        public DesignPropertyDescriptionBuilder WithType(string type)
        {
            this.Type = type;
            return this;
        }

        public DesignPropertyDescriptionBuilder WithBaseType(bool isBaseType)
        {
            this.IsBaseType = isBaseType;
            return this;
        }

        public DesignPropertyDescriptionBuilder WithInterface(bool isInterface)
        {
            this.IsInterface = isInterface;
            return this;
        }

        public DesignPropertyDescriptionBuilder WithCanWrite(bool canWrite)
        {
            this.CanWrite = canWrite;
            return this;
        }

        public DesignPropertyDescriptionBuilder WithIsAutoProperty(bool b)
        {
            this.IsAutoProperty = b;
            return this;
        }

        public IDesignPropertyDescription Build()
        {
            var mock = new Mock<IDesignPropertyDescription>();
            mock.Setup(description => description.Name)
                .Returns(this.Name);
            mock.Setup(description => description.TypeFullName)
                .Returns(TypeFullName.Create(this.Type));
            mock.Setup(description => description.AccessModifier)
                .Returns(AccessModifier.CreatePublic);
            mock.Setup(desc => desc.IsBaseType)
                .Returns(this.IsBaseType);
            mock.Setup(desc => desc.IsInterface)
                .Returns(this.IsInterface);

            mock.Setup(desc => desc.CanWrite).Returns(this.CanWrite);
            mock.Setup(desc => desc.IsAutoProperty).Returns(this.IsAutoProperty);
            return mock.Object;
        }
    }
}
