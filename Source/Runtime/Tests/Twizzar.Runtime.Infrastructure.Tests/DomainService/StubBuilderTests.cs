using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Runtime.Infrastructure.DomainService;

namespace Twizzar.Runtime.Infrastructure.Tests.DomainService;

[TestFixture]
public class StubBuilderTests
{
    private Type _testType;
    private MethodInfo _methodInfo;
    private PropertyInfo _propertyInfo;

    [SetUp]
    public void Setup()
    {
        this._testType = typeof(ITestInterface);
        this._methodInfo = this._testType.GetMethod(nameof(ITestInterface.Add));
        this._propertyInfo = this._testType.GetProperty(nameof(ITestInterface.MyProp));
    }

    [Test]
    public void Method_value_is_setuped_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var builder = sut.Method(this._methodInfo);
        builder.AddMethodValue(5);
        builder.Setup();

        var mock = sut.Build();
        var result = mock.Object.Add(1, 2);

        // assert
        result.Should().Be(5);
    }

    [Test]
    public void Method_delegate_is_setuped_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var builder = sut.Method(this._methodInfo);
        builder.AddMethodValue(new Func<int, int, int>((a, b) => a + b));
        builder.Setup();

        var mock = sut.Build();
        var result = mock.Object.Add(1, 2);

        // assert
        result.Should().Be(3);
    }

    [Test]
    public void Method_callbacks_are_invoked_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();
        var receivedCallbacks = new List<(int, int)>();

        // act
        var builder = sut.Method(this._methodInfo);
        builder.AddMethodCallback(new Action<int, int>((a, b) => receivedCallbacks.Add((a, b))));
        builder.Setup();

        var mock = sut.Build();
        var _ = mock.Object.Add(1, 2);

        // assert
        receivedCallbacks.Should().HaveCount(1);
        receivedCallbacks.Should().Contain((1, 2));
    }

    [Test]
    public void Callbacks_and_return_value_work_together()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();
        var receivedCallbacks = new List<(int, int)>();

        // act
        var builder = sut.Method(this._methodInfo);
        builder.AddMethodValue(42);
        builder.AddMethodCallback(new Action<int, int>((a, b) => receivedCallbacks.Add((a, b))));
        builder.Setup();

        var mock = sut.Build();
        var result = mock.Object.Add(1, 2);

        // assert
        receivedCallbacks.Should().HaveCount(1);
        receivedCallbacks.Should().Contain((1, 2));
        result.Should().Be(42);
    }

    [Test]
    public void Property_value_is_setuped_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        sut.SetupPropertyValue(this._propertyInfo, 5);

        var mock = sut.Build();
        var result = mock.Object.MyProp;

        // assert
        result.Should().Be(5);
    }

    [Test]
    public void Property_delegate_is_setuped_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        sut.SetupPropertyValue(this._propertyInfo, () => 5);

        var mock = sut.Build();
        var result = mock.Object.MyProp;

        // assert
        result.Should().Be(5);
    }

    [Test]
    public void Generic_method_is_setup_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.MyGenericMethod)));
        mBuilder.AddMethodValue(5);
        mBuilder.Setup();
        var result = sut.Build().Object;

        // assert
        result.MyGenericMethod<object>(3).Should().Be(5);
        result.MyGenericMethod<int>(2).Should().Be(5);
    }

    [Test]
    public void Generic_method_callback_is_setup_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();
        var myCallBackList = new List<object>();

        // act
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.MyGenericMethod)));
        mBuilder.AddMethodCallback(new Action<int>(o => myCallBackList.Add(o)));
        mBuilder.Setup();
        var result = sut.Build().Object;
        result.MyGenericMethod<int>(5);

        // assert
        myCallBackList.Should().HaveCount(1).And.AllBeEquivalentTo(5);
    }

    [Test]
    public void Generic_method_with_struct_constrain_setup_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.MyGenericStructMethod)));
        mBuilder.AddMethodValue(5);
        mBuilder.Setup();
        var result = sut.Build().Object;

        // assert
        result.MyGenericStructMethod<int>().Should().Be(5);
    }

    [Test]
    public void Generic_method_with_constrain_setup_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.MyGenericConstrainMethod)));
        mBuilder.AddMethodValue(new List<int>() { 4 });
        mBuilder.Setup();
        var result = sut.Build().Object;

        // assert
        result.MyGenericConstrainMethod<List<int>>().Should().Contain(4);
        result.MyGenericConstrainMethod<ICollection<int>>()
            .Should()
            .BeAssignableTo<List<int>>()
            .Subject.Should()
            .Contain(4);
    }

    [Test]
    public void Generic_method_with_nested_constrain_setup_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.MyGenericListMethod)));
        mBuilder.AddMethodValue(new List<IList<int>>() { new List<int>() { 2 } });
        mBuilder.Setup();
        var result = sut.Build().Object;

        // assert
        result.MyGenericListMethod<int>().Should().BeAssignableTo<IList<IList<int>>>();
        result.MyGenericListMethod<int>().Single().Should().Contain(2);
    }

    [Test]
    public void Generic_method_with_nested_constrain_setup_correctly2()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.MyGenericTupleMethod)));
        mBuilder.AddMethodValue((5, "Test"));
        mBuilder.Setup();
        var result = sut.Build().Object;

        // assert
        result.MyGenericTupleMethod<int, string>().Should().Be((5, "Test"));
    }

    [Test]
    public void Generic_method_with_nested_constrain_setup_correctly3()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var list = new Mock<MyList<int>>().Object;
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.MyGenericMyListMethod)));
        mBuilder.AddMethodValue(list);
        mBuilder.Setup();
        var result = sut.Build().Object;

        // assert
        result.MyGenericMyListMethod<int>(list).Should().Be(list);
    }

    [Test]
    public void Generic_method_with_generic_array_is_setuped_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.MyGenericArrayMethod)));
        mBuilder.AddMethodValue(new[] { 1, 2, 3});
        mBuilder.Setup();
        var result = sut.Build().Object;

        // assert
        result.MyGenericArrayMethod(Array.Empty<int>())
            .Should()
            .BeEquivalentTo(
                new[] { 1, 2, 3 });
    }

    [Test]
    public void Generic_method_with_generic_array_and_delegate_value_is_setuped_correctly()
    {

        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.MyGenericArrayMethod)));
        mBuilder.AddMethodValue(new Func<int[], object>(x => new int[] { 1, 2, 3 }));
        mBuilder.Setup();
        var result = sut.Build().Object;

        // assert
        result.MyGenericArrayMethod(Array.Empty<int>())
            .Should()
            .BeEquivalentTo(
                new[] { 1, 2, 3 });
    }

    [Test]
    public void Generic_method_with_generic_nested_array_is_setuped_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();

        // act
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.MyGenericNestedArrayMethod)));
        mBuilder.AddMethodValue(new[] { new[] { 1, 2, 3 }});
        mBuilder.Setup();
        var result = sut.Build().Object;

        // assert
        result.MyGenericNestedArrayMethod(Array.Empty<int[]>()).Single()
            .Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Test]
    public void Generic_method_with_generic_nested_task_array_is_setuped_correctly()
    {
        // arrange
        var sut = new StubBuilder<ITestInterface>();
        var l = new List<Tuple<string, int>>() { new("a", 5) };


        // act
        var mBuilder = sut.Method(this._testType.GetMethod(nameof(ITestInterface.ComplicatedGeneric)));
        mBuilder.AddMethodValue(l);
        mBuilder.Setup();
        var result = sut.Build().Object;

        // assert
        var t = result.ComplicatedGeneric<string, int>(Task.FromResult<IList<int>>(new List<int>()));
        t.Should().BeEquivalentTo(l);
    }
}

class MyList<T> where T : struct
{

}

interface ITestInterface
{
    int Add(int a, int b);

    int MyProp { get; set; }

    T MyGenericMethod<T>(int a);

    T MyGenericStructMethod<T>() where T : struct;
    T MyGenericConstrainMethod<T>() where T : ICollection<int>;
    IList<IList<T>> MyGenericListMethod<T>() where T : struct;
    ValueTuple<T, K> MyGenericTupleMethod<T, K>() where T : struct;

    MyList<T> MyGenericMyListMethod<T>(MyList<T> param) where T : struct;

    T[] MyGenericArrayMethod<T>(T[] a);
    T[][] MyGenericNestedArrayMethod<T>(T[][] a);

    IList<Tuple<T, K>> ComplicatedGeneric<T, K>(Task<IList<K>> task) where K : struct;

}