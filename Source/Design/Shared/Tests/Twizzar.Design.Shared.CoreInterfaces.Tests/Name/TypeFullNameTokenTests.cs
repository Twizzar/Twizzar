using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Shared.CoreInterfaces.Tests.Name;

[TestFixture]
public class TypeFullNameTokenTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<TypeFullNameToken>()
            .SetupParameter("arrayBrackets", ImmutableArray<string>.Empty)
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Equals_is_implemented_correctly()
    {
        var (ns, typeName, declaringType, genericPostfix, containingText) =
            Build.New<(
                string ns,
                string typeName,
                string declaringType,
                string genericPostfix,
                string containingText)>();

        var bTuples = Build.New<(
            string ns,
            string typeName,
            string declaringType,
            string genericPostfix,
            string containingText)>();

        ImmutableArray<ITypeFullNameToken> CreateRandomTypeFullNameTokens()
        {
            var randomTypeFullName = TypeFullName.Create(RandomTypeFullName().FullName);
            return ImmutableArray<ITypeFullNameToken>.Empty.Add(randomTypeFullName.TypeFullNameToken);
        }

        ImmutableArray<string> CreateRandomBrackets() =>
            ImmutableArray<string>.Empty.Add(RandomString());

        var aTypeFullNameTokens = CreateRandomTypeFullNameTokens();
        var bTypeFullNameTokens = CreateRandomTypeFullNameTokens();

        var aBrackets = CreateRandomBrackets();
        var bBrackets = CreateRandomBrackets();

        var aValues = new object[]
        {
            ns,
            typeName,
            declaringType,
            genericPostfix,
            aTypeFullNameTokens,
            aBrackets,
            containingText,
        }.ToImmutableArray();

        var bValues = new object[]
        {
            bTuples.ns,
            bTuples.typeName,
            bTuples.declaringType,
            bTuples.genericPostfix,
            bTypeFullNameTokens,
            bBrackets,
            bTuples.containingText,
        }.ToImmutableArray();

        TypeFullNameToken Create(params object[] parameters) =>
            new(
                (string)parameters[0],
                (string)parameters[1],
                (string)parameters[2],
                (string)parameters[3],
                (ImmutableArray<ITypeFullNameToken>)parameters[4],
                (ImmutableArray<string>)parameters[5],
                (string)parameters[6]);

        // ignore containing text
        for (int i = 0; i < aValues.Length - 1; i++)
        {
            var values = aValues.SetItem(i, bValues[i]);
            var a = Create(aValues.ToArray());
            var b = Create(values.ToArray());

            Console.WriteLine($"parameter {i}");
            a.Should().NotBeEquivalentTo(b);
        }

        var aa = Create(aValues.ToArray());
        var bb = Create(aValues.ToArray());
        aa.Should().BeEquivalentTo(bb);
    }

    [Test]
    public void METHOD()
    {
        static IEnumerable<ITypeFullNameToken> CreateTokens(int count)
        {
            if (count <= 0)
            {
                return Enumerable.Empty<ITypeFullNameToken>();
            }

            return Enumerable.Range(0, count)
                .Select(
                    i =>
                        new TypeFullNameToken(
                            "Ns",
                            "MyType",
                            Maybe.Some("OuterType+"),
                            Maybe.Some("´"),
                            CreateTokens(i - 1).ToImmutableArray(),
                            ImmutableArray<string>.Empty,
                            RandomString("")));
        }

        var sut = new TypeFullNameToken(
            "NS.",
            "MyType",
            Maybe.Some("OuterType+"),
            Maybe.Some("´"),
            CreateTokens(10).ToImmutableArray(),
            ImmutableArray<string>.Empty,
            "");


        var stopWatch = Stopwatch.StartNew();
        var friendlyTypeFullName = sut.ToFriendlyCSharpFullTypeName();
        stopWatch.Stop();

        Console.WriteLine(stopWatch.ElapsedMilliseconds);
        Console.WriteLine(friendlyTypeFullName);

        stopWatch.ElapsedMilliseconds.Should().BeLessThan(10);

        var t = TypeFullName.CreateFromToken(sut);

        stopWatch.Restart();
        t.GetFriendlyCSharpFullName();
        stopWatch.Stop();
        Console.WriteLine(stopWatch.ElapsedMilliseconds);
    }
}