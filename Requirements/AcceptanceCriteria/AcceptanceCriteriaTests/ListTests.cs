using System;
using System.Collections.Generic;
using FluentAssertions;

using NUnit.Framework;
using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    public class ListTests
    {

        [Test]
        public void Get_List_and_add_some_elements()
        {
            var list = new ItemBuilder<List<int>>().Build();
            list.Count.Should().Be(0);

            list.Add(1);
            list.Count.Should().Be(1);
        }

        [Test]
        public void Get_Dictionary_and_add_some_elements()
        {
            var dict = new ItemBuilder<Dictionary<int, string>>().Build();
            dict.Count.Should().Be(0);

            dict.Add(1, Guid.NewGuid().ToString());
            dict.Count.Should().Be(1);
        }

        [Test]
        public void Class_with_List_dependency_resolved()
        {
            var sut = new ItemBuilder<ListAndDictDependency>().Build();
            sut.Should().NotBeNull();
        }
    }

    public class ListAndDictDependency
    {
        public ListAndDictDependency(List<char> list, Dictionary<int, string> dict)
        {
            
        }
    }
}
