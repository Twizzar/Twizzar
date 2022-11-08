using Moq;
using PotionDeliveryService.Exceptions;
using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests;

[TestFixture]
public partial class DeliveryServiceTests
{
    [Test]
    public void When_trying_to_send_non_potion_throw_InvalidOperationException()
    {
        // arrange
        var deliveryService = new ItemBuilder<DeliveryService>()
            .With(p => p.Ctor.storage.CheckAvailable_Boolean.Value(true))
            .Build();

        var destination = new ItemBuilder<IDestination>().Build();

        // act & assert
        Assert.Throws<InvalidOperationException>(() =>
            deliveryService.Deliver("My Potion", destination));
    }

    [Test]
    public void When_potion_and_its_ingredients_are_not_available_throw_PotionNotAvailableException()
    {
        // arrange
        var potionName = "MyPotion";

        var deliveryService = new DeliveryServiceaa96Builder()
            .With(p => p.Ctor.storage.CheckAvailable_Boolean.Value(false))
            .Build();

        var destination = new ItemBuilder<IDestination>().Build();

        // act and assert
        Assert.Throws<PotionNotAvailableException>(() =>
            deliveryService.Deliver(potionName, destination));
    }

    [Test]
    public void Package_when_potion_in_storage_available_will_be_send_to_destination()
    {
        // arrange
        var (deliveryService, scope) = new DeliveryServiceaa96Builder()
            .BuildWithScope();

        var destinationMock = new Mock<IDestination>();

        var expectedPotion = new Potion(
            scope.Get(p => p.Ctor.potionRecipes.GetPotionName_String),
            scope.Get(p => p.Ctor.potionRecipes.GetPotionRecipe_ValueTupleIIngredientIIngredient).Item1,
            scope.Get(p => p.Ctor.potionRecipes.GetPotionRecipe_ValueTupleIIngredientIIngredient).Item2,
            scope.Get(p => p.Ctor.potionRecipes.GetPotionEffect_IEffect),
            scope.Get(p => p.Ctor.potionRecipes.GetPotionColor_PotionColor));

        // act
        deliveryService.Deliver("MyPotion", destinationMock.Object);

        // assert
        destinationMock.Verify(destination => destination.Receive(It.Is<IPackage<IPotion>>(package => package.UnWrap().First().Equals(expectedPotion))));
    }

    [Test]
    public void Package_when_potion_not_in_storage_available_will_be_send_to_destination()
    {
        // arrange
        var ingredients = new ItemBuilder<IIngredient>()
            .With(p => p.Name.Unique())
            .BuildMany(2);

        var storage = Mock.Of<IStorage>(s =>
            s.CheckAvailable(It.IsAny<string>()) == true &&
            s.CheckAvailable("MyPotion") == false);

        var deliveryService= new ItemBuilder<DeliveryService>()
            .With(p => p.Ctor.storage.Value(storage))
            .With(p => p.Ctor.cauldron.Brew_IPotion.Stub<IPotion>())
            .With(p => p.Ctor.cauldron.Brew_IPotion.Name.Value("MyPotion"))
            .With(p => p.Ctor.potionRecipes.GetPotionRecipe_ValueTupleIIngredientIIngredient.Value((ingredients[0], ingredients[1])))
            .Build();

        var destinationMock = new Mock<IDestination>();

        // act
        deliveryService.Deliver("MyPotion", destinationMock.Object);

        // assert
        destinationMock.Verify(destination => destination.Receive(It.Is<IPackage<IPotion>>(package => package.UnWrap().First().Name == "MyPotion")));
    }
}