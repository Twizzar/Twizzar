using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Test.UnitTest.Helpers;

[TestClass]
public class AccessModifierExtensionsTests
{
    [TestMethod]
    [DataRow(true, false, false, false, MemberModifier.Private)]
    [DataRow(false, true, false, false, MemberModifier.Public)]
    [DataRow(false, false, true, false, MemberModifier.Protected)]
    [DataRow(false, false, false, true, MemberModifier.Internal)]
    public void MapToMemberModifier_returns_correct_memberModifier(bool isPrivate, bool isPublic, bool isProtected, bool isInternal, MemberModifier expected)
    {
        // arrange
        var accessModifier = new AccessModifier(isPrivate, isPublic, isProtected, isInternal);

        // act
        var memberModifier = accessModifier.MapToMemberModifier();

        // assert
        memberModifier.Should().Be(expected);
    }
}