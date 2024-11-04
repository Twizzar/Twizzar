using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;

namespace Twizzar.TestCommon.TypeDescription
{
    /// <summary>
    /// Test class to act and assert base functionality of typeDescription.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypeDescriptionTester<T> where T : ITypeDescription
    {
        private readonly IItemBuilder<T> _builder;

        public TypeDescriptionTester(IItemBuilder<T> builder)
        {
            this._builder = builder;
        }

        public void BaseType_is_set_correctly(Type expectedBaseType)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.BaseType.IsSome.Should().Be(true);
            sut.BaseType.GetValueUnsafe().FullName.Should().Be(expectedBaseType.FullName);
        }

        public void ImplementedInterfaceNames_is_set_correctly(IEnumerable<Type> expectedImplementedInterfaceNames)
        {
            // act
            var sut = this._builder.Build();
            // assert
            sut.ImplementedInterfaceNames.Should()
                .BeEquivalentTo(expectedImplementedInterfaceNames.Select(t => t.FullName));
        }

        public void IsSealed_is_set_correctly(bool expectedIsSealed)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsSealed.Should().Be(expectedIsSealed);
        }

        public void IsStatic_is_set_correctly(bool expectedIsStatic)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsStatic.Should().Be(expectedIsStatic);
        }

        public void IsGeneric_is_set_correctly(bool expectedIsGeneric)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsGeneric.Should().Be(expectedIsGeneric);
        }

        public void IsNested_is_set_correctly(bool expectedIsNested)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsNested.Should().Be(expectedIsNested);
        }

        public void IsArray_is_set_correctly(bool expectedValue)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsArray.Should().Be(expectedValue);
        }

        public void ArrayDimension_is_set_correctly(int[] dimension)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.ArrayDimension.Should().ContainInOrder(dimension);
        }

        public void IsInheritedFromICollection_set_correctly(bool expectedValue)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsInheritedFromICollection.Should().Be(expectedValue);
        }

        #region Nested type: ClassA

        public class ClassA : IA1, IA2, IA3
        {
            /// <inheritdoc />
            public int IB1Prop { get; }

            #region Implementation of IC1

            /// <inheritdoc />
            public int IC1Prop { get; }

            #endregion

            #region Implementation of IA3

            /// <inheritdoc />
            public int IA3Prop { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: GenericClass

        public static class GenericClass<T1, T2>
        {
        }

        #endregion

        #region Nested type: IA1

        public interface IA1
        {
        }

        #endregion

        #region Nested type: IA2

        public interface IA2
        {
        }

        #endregion

        #region Nested type: IA3

        public interface IA3 : IB1
        {
            public int IA3Prop { get; set; }
        }

        public interface IB1 : IC1
        {
            public int IB1Prop { get; }
        }

        public interface IC1
        {
            public int IC1Prop { get; }
        }

        #endregion

        #region Nested type: SealedClass

        public sealed class SealedClass
        {
        }

        #endregion

        #region Nested type: StaticClass

        public static class StaticClass
        {
        }

        #endregion

        #region Nested type: TestClass

        public class TestClass
        {
        }


        public interface IMarkerCollection : ITestCollection { }

        public interface ITestCollection : ICollection
        {

        }

        public class TestCollection : IMarkerCollection
        {
            #region Implementation of IEnumerable

            /// <inheritdoc />
            public IEnumerator GetEnumerator() => throw new NotImplementedException();

            #endregion

            #region Implementation of ICollection

            /// <inheritdoc />
            public void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public int Count { get; }

            /// <inheritdoc />
            public bool IsSynchronized { get; }

            /// <inheritdoc />
            public object SyncRoot { get; }

            #endregion
        }

        #endregion

    }
}