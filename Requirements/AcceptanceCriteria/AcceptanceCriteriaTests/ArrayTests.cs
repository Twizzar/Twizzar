using FluentAssertions;

using NUnit.Framework;
using System;
using System.Collections.Generic;
using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    public class ArrayTests
    {
        [Test]
        public void Get_array()
        {
            var list = new ItemBuilder<int[,]>().Build();
            var list2 = new ItemBuilder<int[]>().Build();
            var list3 = new ItemBuilder<int[][,,]>().Build();
            var list4 = new ItemBuilder<int[,][,,][]>().Build();

            list.Should().HaveCount(0);
            list2.Should().HaveCount(0);
            list3.Should().HaveCount(0);
            list4.Should().HaveCount(0);
        }

        [Test]
        public void Class_with_List_dependency_resolved()
        {
            var sut = new ItemBuilder<ArrayDependency>().Build();
            sut.Should().NotBeNull();
            sut.CtorChar.Should().NotBeNull();
            sut.ArrayProp.Should().BeNull();
        }
    }

    public class ArrayDependency
    {
        public char[][] CtorChar { get; }
        public string[,] CtorString { get; }
        public ArrayDependency(char[][] v1, string[,] v2)
        {
            this.CtorChar = v1;
            this.CtorString = v2;
        }
        public List<int[]> ListProp { get; set; }
        public IComparable<(char[], string)> ComProp { get; set; }
        public int[] ArrayProp { get; set; }
    }
}
