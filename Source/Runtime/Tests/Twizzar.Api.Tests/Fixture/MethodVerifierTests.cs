using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Fixture;
using Twizzar.Fixture.Member;
using Twizzar.Fixture.MethodVerifier;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using ViCommon.Functional.Monads.MaybeMonad;

using static Twizzar.Api.Tests.Fixture.IMethodTestPathDummyProvider;
using TzVoid = Twizzar.Fixture.TzVoid;
using VerificationException = Twizzar.Fixture.VerificationException;

namespace Twizzar.Api.Tests.Fixture;

public static class MethodVerifierExtensions
{
    public static IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, MethodBasetypeMemberPath<IMethodTest, IList<int>, IList<int>>> WhereCount(
        this IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, MethodBasetypeMemberPath<IMethodTest, IList<int>, IList<int>>> self,
        Expression<Func<int, bool>> predicate) => 
            self.WhereParamIs<int>("count", predicate);

    public static IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, MethodBasetypeMemberPath<IMethodTest, IList<int>, IList<int>>> WhereCount(
        this IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, MethodBasetypeMemberPath<IMethodTest, IList<int>, IList<int>>> self, int value) =>
            self.WhereParamIs<int>("count", value);

    public static IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, MethodBasetypeMemberPath<IMethodTest, IList<int>, IList<int>>>
        WhereCount(this IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, MethodBasetypeMemberPath<IMethodTest, IList<int>, IList<int>>> self, Func<IMethodTestPathDummyProvider, MemberPath<IMethodTest, int>> selector) =>
            self.WhereParamIs<int>("count", selector);


    public static IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder> WhereCount(
        this IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder> self,
        Expression<Func<int, bool>> predicate) =>
        self.WhereParamIs<int>("count", predicate);

    public static IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder> WhereCount(
        this IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder> self, int value) =>
        self.WhereParamIs<int>("count", value);

    public static IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder>
        WhereCount(this IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder> self, Func<IMethodTestPathDummyProvider, MemberPath<IMethodTest, int>> selector) =>
        self.WhereParamIs<int>("count", selector);

    public static IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngine_ICylinderMemberPath> WhereCount(
        this IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngine_ICylinderMemberPath> self,
        Expression<Func<int, bool>> predicate) =>
        self.WhereParamIs<int>("count", predicate);

    public static IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngine_ICylinderMemberPath> WhereCount(
        this IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngine_ICylinderMemberPath> self, int value) =>
        self.WhereParamIs<int>("count", value);

    public static IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngine_ICylinderMemberPath>
        WhereCount(this IMethodVerifier<IMethodTest,IMethodTestPathDummyProvider, GetEngine_ICylinderMemberPath> self, Func<IMethodTestPathDummyProvider, MemberPath<IMethodTest, int>> selector) =>
        self.WhereParamIs<int>("count", selector);
}

public class MethodVerifierTests
{
    [Test]
    public void Called_simple_method_does_not_throw_when_method_is_called()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .GetNumber_System_Int32;

        var methodTestMock = Mock.Of<IMethodTest>();
        methodTestMock.GetNumber();

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, MethodBasetypeMemberPath<IMethodTest, IList<int>, IList<int>>>(
            methodPath,
            instanceCacheQuery);
        
        // act
        sut.CalledAtLeastOnce();
    }

    [Test]
    public void Called_simple_method_does_not_throws_when_method_is_not_called()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .GetNumber_System_Int32;

        var methodTestMock = Mock.Of<IMethodTest>();

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, MethodBasetypeMemberPath<IMethodTest, IList<int>, IList<int>>>(
            methodPath,
            instanceCacheQuery);
        
        // act
        var verify = () => sut.CalledAtLeastOnce();

        // assert
        verify.Should().Throw<VerificationException>();
    }

    [Test]
    public void Called_method_with_parameter_does_throws_when_method_is_called()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .GetEngine_ICylinder;

        var methodTestMock = Mock.Of<IMethodTest>();
        methodTestMock.GetEngine(5);

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, MethodBasetypeMemberPath<IMethodTest, IList<int>, IList<int>>>(
            methodPath,
            instanceCacheQuery);

        // act
        var verify = () => sut
            .WhereCount(3)
            .CalledAtLeastOnce();

        // assert
        verify.Should().Throw<VerificationException>()
            .And.Message.Should().Contain("no invocation was performed.");
    }

    [Test]
    public void Called_method_with_parameter_does_not_throws_when_method_is_not_called()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .GetEngine_ICylinder;

        var methodTestMock = Mock.Of<IMethodTest>();
        methodTestMock.GetEngine(3);

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, IMethodTestPathDummyProvider.GetEngine_ICylinderMemberPath>(
            methodPath,
            instanceCacheQuery);

        // act
        var verify = () => sut
            .WhereCount(3)
            .CalledAtLeastOnce();

        // assert
        verify.Should().NotThrow();
    }

    [Test]
    public void Called_method_with_parameters_description_does_not_throw_on_verification()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .GetEngine__Int32;

        var methodTestMock = Mock.Of<IMethodTest>();
        methodTestMock.GetEngine(3);

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder>(
            methodPath,
            instanceCacheQuery);

        // act
        var verify = () => sut
            .WhereCount(3)
            .CalledAtLeastOnce();

        // assert
        verify.Should().NotThrow();
    }

    [Test]
    public void Not_called_method_with_parameters_description_throws_on_verification()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .GetEngine__Int32;

        var methodTestMock = Mock.Of<IMethodTest>();
        methodTestMock.GetEngine();

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder>(
            methodPath,
            instanceCacheQuery);

        // act
        var verify = () => sut
            .WhereCount(3)
            .CalledAtLeastOnce();

        // assert
        verify.Should().Throw<VerificationException>();
    }

    [Test]
    public void When_no_method_exists_with_verified_parameter_throw_exception()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .GetEngine__Int32;

        var methodTestMock = Mock.Of<IMethodTest>();
        methodTestMock.GetEngine();

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder>(
            methodPath,
            instanceCacheQuery);

        // act
        var verify = () => sut
            .WhereParamIs<float>("DoesNotExists", f => true)
            .CalledAtLeastOnce();

        // assert
        verify.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Verification_on_called_void_method_dont_throw()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .VoidMethod;

        var methodTestMock = Mock.Of<IMethodTest>();
        methodTestMock.VoidMethod();

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder>(
            methodPath,
            instanceCacheQuery);

        // act
        var verify = () => sut
            .CalledAtLeastOnce();

        // assert
        verify.Should().NotThrow();
    }

    [Test]
    public void Verification_on_not_called_void_method_throw()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .VoidMethod;

        var methodTestMock = Mock.Of<IMethodTest>();

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, MethodBasetypeMemberPath<IMethodTest, TzVoid, TzVoid>>(
            methodPath,
            instanceCacheQuery);

        // act
        var verify = () => sut
            .CalledAtLeastOnce();

        // assert
        verify.Should().Throw<VerificationException>();
    }

    [Test]
    public void Exception_message_is_set_correctly()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .GetEngine__Int32;

        var methodTestMock = Mock.Of<IMethodTest>();

        for (int i = 0; i < 3; i++)
        {
            methodTestMock.GetEngine(5);
        }

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, GetEngineMemberPath_ICylinder>(
            methodPath,
            instanceCacheQuery);

        // act
        var verify = () => sut
            .WhereParamIs<int>("count", 5)
            .Called(5);

        // assert
        verify.Should().Throw<VerificationException>()
            .And.Message.Should().Contain("Expected exactly 5 invocations on IMethodTest, but 3 invocations where performed.");
    }

    [Test]
    public void Verification_of_simple_generics_works_correctly()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .GenericMethodT;

        var methodTestMock = Mock.Of<IMethodTest>();
        methodTestMock.GenericMethod<int>();

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, GenericMethodTMemberPath>(
            methodPath,
            instanceCacheQuery);

        // act
        var verify = () => sut
            .Called(1);

        // assert
        verify.Should().NotThrow();
    }

    [Test]
    public void Verification_of_complex_generics_works_correctly()
    {
        // arrange
        var methodPath = new IMethodTestPathDummyProvider()
            .ComplexGenericMethodT;

        var methodTestMock = Mock.Of<IMethodTest>();
        methodTestMock.ComplexGenericMethod<int>(null);
        methodTestMock.ComplexGenericMethod<char>(null);

        var instanceCacheQuery = Mock.Of<IInstanceCacheQuery>(
            query => query.GetInstance(It.IsAny<string>()) == Maybe.Some<object>(methodTestMock));

        var sut = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, ComplexGenericMethodTMemberPath>(
            methodPath,
            instanceCacheQuery);

        var sut2 = new MethodVerifier<IMethodTest, IMethodTestPathDummyProvider, ComplexGenericMethodTMemberPath>(
            methodPath,
            instanceCacheQuery);

        // act
        sut
            .WhereParamIs<int[]>("param", l => true)
            .Called(1);

        sut
            .WhereParamIs<char[]>("param", l => true)
            .Called(1);

        sut2
            .Called(2);

        // assert
        Assert.Pass();
    }
}