using System.Linq;

using FluentAssertions;

using NUnit.Framework;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.Infrastructure.Tests.PathTree
{
    [TestFixture]
    public class PathNodeTests
    {
        [Test]
        public void Non_duplicated_path_generates_correct_tree()
        {
            var paths = Enumerable.Range(0, 10)
                .Select(i => Enumerable.Range(0, 10).Select(k => $"{i}-{k}"))
                .ToList();

            var root = PathNode.ConstructRoot(paths);

            root.Parent.Should().Be(Maybe.None<IPathNode>());
            root.Children.Should().HaveCount(10);

            root.Children.Keys.Should().BeEquivalentTo(root.Children.Values.Select(node => node.MemberName));
            root.Children.Keys.Should().BeEquivalentTo(paths.Select(tuple => tuple.First()));
        }

        [Test]
        public void ToString_is_implemented_correctly()
        {
            var paths = Enumerable.Range(0, 3)
                .Select(i => Enumerable.Range(0, 3).Select(k => $"{i}-{k}"))
                .ToList();

            var expected = @$"
⊢0-0
  ⊢0-1
    ⊢0-2
⊢1-0
  ⊢1-1
    ⊢1-2
⊢2-0
  ⊢2-1
    ⊢2-2";

            var root = PathNode.ConstructRoot(paths);
            root.ToString().Trim().Should().Be(expected.Trim());
        }

        [Test]
        public void Paths_with_same_same_get_merged_into_the_same_node()
        {
            var paths = new[]
            {
                new[] { "a", "b", "c" },
                new[] { "a", "d", "c" },
                new[] { "a", "b", "f" },
            };

            var root = PathNode.ConstructRoot(paths);


            root["a"].Children.Should().HaveCount(2);
            root["a"]["b"].Children.Should().HaveCount(2);
            root["a"]["d"].Children.Should().HaveCount(1);
            root["a"]["b"]["c"].Children.Should().BeEmpty();
        }

        [Test]
        public void CountDuplicates_returns_the_correct_amount_of_duplicates()
        {
            var paths = new[]
            {
                new[] { "a", "a", "a" },
            };

            var root = PathNode.ConstructRoot(paths);

            root["a"].CountMemberNameDuplicates().Should().Be(0);
            root["a"]["a"].CountMemberNameDuplicates().Should().Be(1);
            root["a"]["a"]["a"].CountMemberNameDuplicates().Should().Be(2);
        }

        [Test]
        public void Two_different_nodes_are_not_equal()
        {
            var paths1 = new[]
            {
                new[] { "a", },
                new[] { "b", "b" },
            };

            var paths2 = new[]
            {
                new[] { "a", },
                new[] { "b", "b", "c" },
            };

            var root1 = PathNode.ConstructRoot(paths1);
            var root2 = PathNode.ConstructRoot(paths2);

            root1.Equals(root2).Should().BeFalse();
            root1.GetHashCode().Should().NotBe(root2.GetHashCode());
        }

        [Test]
        public void Two_same_nodes_are_equal()
        {
            var paths1 = new[]
            {
                new[] { "a", },
                new[] { "b", "b" },
            };

            var paths2 = new[]
            {
                new[] { "a", },
                new[] { "b", "b" },
            };

            var root1 = PathNode.ConstructRoot(paths1);
            var root2 = PathNode.ConstructRoot(paths2);

            root1.Equals(root2).Should().BeTrue();
            root1.GetHashCode().Should().Be(root2.GetHashCode());
        }

        [TestCaseSource(nameof(MaxDephtCase))]

        public void MaxDepth_is_calculated_correctly(string[][] paths, int maxDepth)
        {
            var root = PathNode.ConstructRoot(paths);

            root.MaxDepth.Should().Be(maxDepth);
        }

        static object[] MaxDephtCase =
        {
            new object[] { 
                new[]
                {
                    new[] { "a", },
                    new[] { "a", "b", "c", "d" },
                },
                5
            },
            new object[] {
                new[]
                {
                    new[] { "a", "b" },
                    new[] { "a" },
                },
                3
            },
            new object[] {
                new[]
                {
                    new[] { "a", },
                },
                2
            },
        };
    }
}