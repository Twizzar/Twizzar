namespace PotionDeliveryService.Tests;

[TestFixture]
public partial class BaseFeaturesTests
{
    [Test]
    public void Build_Items()
    {
        // Builder class for building any type.
        // Helpful for accessing sut and input values for method under test.
        var potion = new ItemBuilder<Potion>().Build();

        Assert.That(potion, Is.AssignableTo<Potion>());

        // asking for a new instance.
        var potion2 = new ItemBuilder<Potion>().Build();
        Assert.That(potion2, Is.Not.EqualTo(potion));
        
        // same valid for build with custom builder:
        var bluePotion = new BluePotionBuilder();
        var bluePotion2 = new BluePotionBuilder();
        Assert.That(bluePotion, Is.Not.EqualTo(bluePotion2));
    }

    [Test]
    public void Build_a_new_class()
    {
        // default: largest public ctor will be chosen.
        // if no public ctor available, largest non public ctor will be chosen.
        // all dependencies in ctor will be resolved with default configuration.
        var potion = new ItemBuilder<Potion>().Build();

        // others will not be touched.
        Assert.That(potion.Price, Is.EqualTo(default(int)));

        // if defined by custom builder, value will be set once:
        var bluePotion = new BluePotionBuilder().Build();
        Assert.That(bluePotion.Price, Is.EqualTo(42));
    }

    [Test]
    public void Build_New_BaseTypes()
    {
        // supported base types:
        // numeric types like int, long, double, float, single, etc.
        // string, char
        // bool

        // default: unique
        var intValue = new ItemBuilder<int>().Build();
        var intValue2 = new ItemBuilder<int>().Build();
        Assert.That(intValue, Is.Not.EqualTo(intValue2));

        // string: GUID
        // bool: true
        // numeric value: unique value

        // base types dependency in ctor will be resolved unique
        var potion = new ItemBuilder<Potion>().Build();
        var potion2 = new ItemBuilder<Potion>().Build();
        Assert.That(potion.Name, Is.Not.EqualTo(potion2.Name));

        // others will not be touched.
        Assert.That(potion.Price, Is.EqualTo(default(int)));

        // values can be set:
        var bluePotion = new BluePotionBuilder().Build();
        Assert.That(bluePotion.Color, Is.EqualTo(PotionColor.Blue));
        Assert.That(bluePotion.Price, Is.EqualTo(42));
    }

    [Test]
    public void Build_new_Interface()
    {
        // default: mock instance which is not further configured.
        var iPotion = new ItemBuilder<IPotion>().Build();
        Assert.That(iPotion.Ingredient1, Is.EqualTo(default));

        // dependency in ctor will be resolved as mock
        var potion = new ItemBuilder<Potion>().Build();
        Assert.That(potion.Ingredient1, Is.AssignableTo<IIngredient>());
        Assert.DoesNotThrow(() => Mock.Get(potion.Ingredient1));

        // others will not be touched.
        Assert.That(potion.Price, Is.EqualTo(default(int)));
    }


    [Test]
    public void Enum_is_BaseType_as_well()
    {
        // default: unique
        var enum1 = new ItemBuilder<PotionColor>().Build();
        var enum2 = new ItemBuilder<PotionColor>().Build();
        var enum3 = new ItemBuilder<PotionColor>().Build();
        var enum1Again = new ItemBuilder<PotionColor>().Build();

        Assert.That(new[] {enum1, enum2, enum3}, Is.Unique);

        // because the PotionColors has only 3 enum vales (Blue,Red,Purple) the fifth creation will be the same as the first.
        Assert.That(enum1, Is.EqualTo(enum1Again));
    }

    [Test]
    public void Structs_are_supported_as_well()
    {
        // default: same behavior as classes
        // structs always have a public empty ctor, which will be chosen if
        // no other public ctor is available.
        var testStruct = new ItemBuilder<TestStruct>().Build();
        Assert.That(testStruct, Is.AssignableTo<TestStruct>());
        Assert.That(testStruct.X, Is.Not.EqualTo(testStruct.Y));
    }

    [Test]
    public void Build_many()
    {
        // resolves the asked configuration n times:
        var potions = new ItemBuilder<Potion>().BuildMany(5);
        Assert.That(potions, Has.Count.EqualTo(5));
        Assert.That(potions, Is.Unique);
        
        // same can be done with specific custom builder:
        var bluePotions = new BluePotionBuilder().BuildMany(50);
        Assert.That(bluePotions, Has.Count.EqualTo(50));
        Assert.That(bluePotions, Is.Unique);
    }

    [Test]
    public void Build_with_scope()
    {
        // When BuildWithScope is used a tuple with the built item and the scope is returned.
        var (potion, scope) = new BluePotionBuilder().BuildWithScope();

        // The scope can be used to get dependencies which are resolved and set by the ItemBuilder.
        var potionColor = scope.Get(p => p.Ctor.color);
        Assert.That(potionColor, Is.EqualTo(potion.Color));

        // It is also possible to get a dependency as an mock
        var ingredientMock = scope.GetAsMoq(p => p.Ctor.ingredient1);
        ingredientMock
            .Setup(ingredient => ingredient.Price)
            .Returns(5);

        Assert.That(potion.Ingredient1.Price, Is.EqualTo(5));

        // It is planned to support other mocking frameworks in the future.

        // WithScope also works with Many
        var potionsAndScope = new BluePotionBuilder().BuildManyWithScope(5);

        // A list of tuples is returned
        Assert.That(potionsAndScope[0].Instance, Is.Not.EqualTo(potionsAndScope[^1].Instance));

        foreach (var (instance, itemScope) in potionsAndScope)
        {
            var ingredientMock2 = itemScope.GetAsMoq(p => p.Ctor.ingredient1);
            ingredientMock2.VerifyNoOtherCalls();
        }
    }

    [Test]
    public void Configure_member_in_code()
    {
        // it is possible to directly configure members.
        var bluePotion = new ItemBuilder<Potion>()
            .With(p => p.Ctor.color.Value(PotionColor.Blue))
            .Build();

        Assert.That(bluePotion.Color, Is.EqualTo(PotionColor.Blue));
        
        // other members will still be resolved with the default behaviour.
        Assert.That(bluePotion.Ingredient1, Is.AssignableTo<IIngredient>());

        // It is possible to directly configure dependencies of dependencies.
        bluePotion = new ItemBuilder<Potion>()
            .With(p => p.Ctor.ingredient1.Name.Value("Water"))
            .Build();

        Assert.That(bluePotion.Ingredient1.Name, Is.EqualTo("Water"));

        // A member can be set to:
        // Value:       Set the member to a given value
        // Unique:      Only for base types, return a unique value
        // Undefined:   Not for constructor parameters, do not set this member
        // Stub:        Change the type of the member to a stub (will resolved as an Mock)
        // InstanceOf:  Change the type of the member (A instance of a class or struct will be resolved with a constructor)
        var potions = new ItemBuilder<Potion>()
            .With(p => p.Price.Unique())
            .With(p => p.Ctor.ingredient1.Stub<IPotion>())
            .With(p => p.Ctor.ingredient2.InstanceOf<Potion>())
            .BuildMany(2);

        Assert.That(potions.Select(potion => potion.Price), Is.Unique);
        Assert.That(potions[0].Ingredient1, Is.InstanceOf<IPotion>());
        Assert.That(potions[0].Ingredient2, Is.InstanceOf<Potion>());

        // Because Ingredient1 it is a stub a mock with no setups will be resolved.
        Assert.That(potions[0].Ingredient1.Name, Is.Null);

        // Because Ingredient2 is a instance a constructor will be called.
        Assert.That(potions[0].Ingredient2.Name, Is.Not.Null);

        // It is also possible to extend a custom builder
        var manaPotion = new BluePotionBuilder()
            .With(p => p.Ctor.effect.Name.Value("Restores Mana"))
            .Build();

        Assert.That(manaPotion.Color, Is.EqualTo(PotionColor.Blue));
        Assert.That(manaPotion.Effect.Name, Is.EqualTo("Restores Mana"));
    }
}


public struct TestStruct
{
    public TestStruct(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }

    public double X { get; }
    public double Y { get; private set; }
}