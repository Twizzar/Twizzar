using System;

using Moq;
using Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.TestCommon.TypeDescription
{
    public class DesignFieldDescriptionBuilder
    {
        public string Type { get; private set; }
        public string Name { get; private set; }
        public bool IsBaseType { get; private set; }
        public bool IsInterface { get; private set; }
        public bool IsBackingfield { get; private set; }
        public bool IsClass { get; private set; }
        public bool IsStruct { get; private set; }
        private Maybe<IPropertyDescription> _backingFieldProperty = Maybe.None();

        public DesignFieldDescriptionBuilder()
        {
            this.Name = Guid.NewGuid().ToString();
            this.Type = Guid.NewGuid().ToString();
        }

        public DesignFieldDescriptionBuilder WithName(string name)
        {
            this.Name = name;
            return this;
        }

        public DesignFieldDescriptionBuilder WithType(string type)
        {
            this.Type = type;
            return this;
        }

        public DesignFieldDescriptionBuilder WithBaseType(bool isBaseType)
        {
            this.IsBaseType = isBaseType;
            return this;
        }

        public DesignFieldDescriptionBuilder WithInterface(bool isInterface)
        {
            this.IsInterface = isInterface;
            return this;
        }

        public DesignFieldDescriptionBuilder WithBackingfield(bool isBackingfield)
        {
            this.IsBackingfield = isBackingfield;
            return this;
        }

        public DesignFieldDescriptionBuilder WithIsClass(bool b)
        {
            this.IsClass = b;
            return this;
        }

        public DesignFieldDescriptionBuilder WithIsStruct(bool b)
        {
            this.IsStruct = b;
            return this;
        }

        public DesignFieldDescriptionBuilder WithNoneWritableBackingfield()
        {
            var mock = new Mock<IPropertyDescription>();
            mock.Setup(p => p.CanWrite).Returns(false);

            this._backingFieldProperty = Maybe.Some<IPropertyDescription>(mock.Object);
            return this;
        }

        public IDesignFieldDescription Build()
        {
            var mock = new Mock<IDesignFieldDescription>();
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
            mock.Setup(desc => desc.IsBackingField)
                .Returns(this.IsBackingfield);
            mock.Setup(desc => desc.IsClass)
                .Returns(this.IsClass);
            mock.Setup(desc => desc.IsStruct)
                .Returns(this.IsStruct);
            mock.Setup(desc => desc.BackingFieldProperty).Returns(this._backingFieldProperty);
            return mock.Object;
        }
    }
}
