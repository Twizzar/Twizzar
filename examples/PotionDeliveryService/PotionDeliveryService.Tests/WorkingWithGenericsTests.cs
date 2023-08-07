namespace PotionDeliveryService.Tests;

public partial class WorkingWithGenericsTests
{
    [Test]
    public void Setup_method_with_a_typeParameter_as_return_type()
    {
        // if the method T SimpleGenericMethod<T>() is setuped with Twizzar
        // the Value expects a object as parameter because every type is assignable to T.
        var sut = new ItemBuilder<IGenericExample>()
            .With(p => p.SimpleGenericMethodT.Value(5))
            .Build();

        // Under the hood, method with the type parameter T = int and T = objects get mocked.
        Assert.AreEqual(5, sut.SimpleGenericMethod<int>());
        Assert.AreEqual(5, sut.SimpleGenericMethod<object>());

        // all other type parameters will return the default value (default(T))
        Assert.AreEqual(null, sut.SimpleGenericMethod<string>());
    }

    [Test]
    public void Naming_of_generic_methods()
    {
        // To discriminate between different method with the same name but different type parameters.
        // The type parameter names will be post fixed to the method name.
        var sut = new ItemBuilder<IGenericExample>()
            .With(p => p.MyMethod.Value(5))
            .With(p => p.MyMethodT.Value("test"))
            .With(p => p.MyMethodTK.Value((3, 4f)))
            .Build();

        Assert.AreEqual(5, sut.MyMethod());
        Assert.AreEqual("test", sut.MyMethod<string>());
        Assert.AreEqual((3, 4f), sut.MyMethod<int, float>());
    }

    [Test]
    public void Setup_value_delegate()
    {
        var sut = new ItemBuilder<IGenericExample>()
            .With(p => p.CreateListT.Value<int>(items => items.ToList()))
            .Build();

        var list = sut.CreateList(1, 2, 3);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
    }

    [Test]
    public void Setup_a_callback()
    {
        var myList = new List<int[]>();

        var sut = new ItemBuilder<IGenericExample>()
            .With(p => p.CreateListT.Callback<int>(items => myList.Add(items)))
            .Build();

        sut.CreateList(1);

        Assert.Contains(1, myList.Single());
    }

    [Test]
    public void Verify_parameter()
    {
        var sut = new IGenericExampleBuilder()
            .Build(out var context);

        sut.CreateList(1, 2, 3);

        context.Verify(p => p.CreateListT)
            .WhereItemsIs<int>(items => items.Contains(1) && items.Contains(2) && items.Contains(3))
            .Called(1);
    }

    [Test]
    public void Playground()
    {
        var sut = new ItemBuilder<IGenericExample>()
            .Build();
    }
}

public interface IGenericExample
{

    int MyMethod();

    T MyMethod<T>();

    (T, K) MyMethod<T, K>();

    T SimpleGenericMethod<T>();

    IList<T> CreateList<T>(params T[] items);
}