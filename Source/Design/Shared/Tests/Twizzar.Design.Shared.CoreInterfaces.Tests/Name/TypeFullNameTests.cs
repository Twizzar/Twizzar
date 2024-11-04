using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using ViCommon.Functional.Monads.MaybeMonad;

#pragma warning disable S125 // Sections of code should not be commented out

namespace Twizzar.Design.CoreInterfaces.Tests.Name
{
    [TestFixture]
    public class TypeFullNameTests
    {
        public static IEnumerable<object[]> ValidTypes
        {
            get
            {
                yield return new object[] { typeof(System.Collections.Generic.List<Int64>) };
                yield return new object[] { typeof(System.UInt32) };
                yield return new object[] { typeof(Maybe<List<string>>) };
                yield return new object[] { typeof(int[]) };
                yield return new object[] { typeof(int[][]) };
                yield return new object[] { typeof((int[], string)) };
            }
        }

        public static IEnumerable<object[]> ArrayTypes
        {
            get
            {
                yield return new object[] { typeof(int[]), new List<int> {1} };
                yield return new object[] { typeof(int[,]), new List<int> { 2 } };
                yield return new object[] { typeof(int[,,]), new List<int> { 3 } };
                yield return new object[] { typeof(int[,,][]), new List<int> { 1, 3 } };
                yield return new object[] { typeof(int[,,][][]), new List<int> { 1,1,3 } };
                yield return new object[] { typeof(int[,,][][,]), new List<int> { 2, 1, 3 } };
            }
        }

        public static IEnumerable<object[]> InvalidTypes
        {
            get
            {
                yield return new object[] { "System<int>.Test" };
                yield return new object[] { "Test/Type" };
                yield return new object[] { "System..Type" };
            }
        }

        public static IEnumerable<object[]> CSharpTypeFullNames
        {
            get
            {
                yield return new object[] { "System.Tuple<System.Int32>", typeof(Tuple<int>).FullName, };
                yield return new object[] { "System.Nullable<System.Int32>", typeof(int?).FullName, };
                yield return new object[] { "System.Collections.Generic.List<System.Int32[]>", typeof(List<int[]>).FullName, };
                yield return new object[] { "System.Int32[][]", typeof(int[][]).FullName, };
                yield return new object[] { "System.Int32[,]", typeof(int[,]).FullName, };
                yield return new object[]
                {
                    "System.Collections.Generic.IEnumerable<Twizzar.Design.CoreInterfaces.Tests.Name.TypeFullNameTests>",
                    typeof(IEnumerable<TypeFullNameTests>).FullName,
                };
                yield return new object[]
                {
                    "System.Tuple<System.String, System.Tuple<System.Int32, System.Int32>>",
                    typeof(Tuple<string, Tuple<int, int>>).FullName,
                };
            }
        }

        public static IEnumerable<object[]> CSharpTypes
        {
            get
            {
                yield return new object[] { "Tuple<Int32>", typeof(Tuple<int>).FullName, };
                yield return new object[] { "Nullable<Int32>", typeof(int?).FullName, };
                yield return new object[]
                {
                    "IEnumerable<TypeFullNameTests>",
                    typeof(IEnumerable<TypeFullNameTests>).FullName,
                };
                yield return new object[]
                {
                    "Tuple<String, Tuple<Int32, Int32>>",
                    typeof(Tuple<string, Tuple<int, int>>).FullName,
                };
            }
        }

        [TestCaseSource(nameof(ValidTypes))]
        public void CrateFromType_GetNameSpace_and_GetTypeName_return_the_correct_result(Type type)
        {
            var sut = TypeFullName.CreateFromType(type);

            sut.GetNameSpace().Should().Be(type.Namespace);
            sut.GetTypeName().Should().Be(type.Name);
        }

        [TestCaseSource(nameof(ArrayTypes))]
        public void CreateFromType_Arrays_Correct_FullName(Type type, List<int> structure)
        {
            var sut = TypeFullName.CreateFromType(type);

            sut.IsArray().Should().BeTrue();
            sut.FullName.Should().Be(type.FullName);
            sut.ArrayDimension().Should().ContainInOrder(structure);
        }

        [TestCaseSource(nameof(ValidTypes))]
        public void Crate_GetNameSpace_and_GetTypeName_return_the_correct_result(Type type)
        {
            var sut = TypeFullName.Create(type.FullName);

            sut.GetNameSpace().Should().Be(type.Namespace);
            sut.GetTypeName().Should().Be(type.Name);
        }

        [TestCaseSource(nameof(CSharpTypeFullNames))]
        public void GetFriendlyCSharpName_returns_the_expected_result(string expectedTypeName, string typeFullName)
        {
            // arrange
            //var expectedTypeName = "System.Tuple<System.Int32>";
            var sut = TypeFullName.Create(typeFullName);

            // act
            var name = sut.GetFriendlyCSharpFullName();

            // assert
            name.Should().Be(expectedTypeName);
        }

        [TestCaseSource(nameof(CSharpTypes))]
        public void GetFriendlyCSharpTypeName_returns_the_expected_result(string expectedTypeName, string typeFullName)
        {
            // arrange
            //var expectedTypeName = "System.Tuple<System.Int32>";
            var sut = TypeFullName.Create(typeFullName);

            // act
            var name = sut.GetFriendlyCSharpTypeName();

            // assert
            name.Should().Be(expectedTypeName);
        }
    }
}
