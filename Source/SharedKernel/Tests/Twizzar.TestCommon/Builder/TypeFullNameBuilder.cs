using System;
using Moq;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional.Monads.MaybeMonad;

// User pattern matching, we cannot use pattern matching in a Linq Expression
#pragma warning disable IDE0038

namespace Twizzar.TestCommon.Builder
{
    public class TypeFullNameBuilder
    {
        private readonly string _typeFullName;
        private readonly bool _isNullable;
        private readonly string _typeName;
        private readonly Maybe<ITypeFullName> _nullableGenericUnderlyingType = Maybe.None();

        public TypeFullNameBuilder(string typeFullName)
        {
            this._typeFullName = typeFullName;
            this._typeName = typeFullName;
        }

        public TypeFullNameBuilder(Type type)
        {
            this._typeFullName = type.FullName;
            this._isNullable = type.IsNullableType();
            this._typeName = type.Name;
            this._nullableGenericUnderlyingType = Maybe.ToMaybe(Nullable.GetUnderlyingType(type))
                .Map(type1 => new TypeFullNameBuilder(type1).Build());
        }

        public ITypeFullName Build() =>
            Mock.Of<ITypeFullName>(name => 
                name.FullName == this._typeFullName &&
                name.IsNullable() == this._isNullable &&
                name.GetTypeName() == this._typeName &&
                name.NullableGetUnderlyingType() == this._nullableGenericUnderlyingType &&
                name.GetFriendlyCSharpTypeName() == this._typeFullName &&
                name.Equals(It.Is<ITypeFullName>(fname => fname.FullName == this._typeFullName)) &&
                name.Equals(It.Is<object>(obj => (obj is ITypeFullName) && ((ITypeFullName)obj).FullName == this._typeFullName)));
    }
}
