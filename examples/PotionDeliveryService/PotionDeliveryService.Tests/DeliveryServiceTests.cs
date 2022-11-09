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

        // create a delivery service where the potion is available but the storage returns always a ingredient and not a potion.
        var deliveryService = new ItemBuilder<DeliveryService>()
            .With(p => p.Ctor.storage.CheckAvailable_Boolean.Value(true))
            .With(p => p.Ctor.storage.Take_IIngredient.Stub<IIngredient>())
            .Build();

        // create a stub for the destination
        var destination = new ItemBuilder<IDestination>().Build();

        // act & assert

        // an exception should be thrown because the storage did not return a potion.
        Assert.Throws<InvalidOperationException>(() =>
            deliveryService.Deliver("My Potion", destination));
    }

    [Test]
    public void When_potion_and_its_ingredients_are_not_available_throw_PotionNotAvailableException()
    {
        // arrange
        const string potionName = "MyPotion";

        // create a delivery service where the nothing is available in the storage.
        var deliveryService = new DeliveryServiceaa96Builder()
            .With(p => p.Ctor.storage.CheckAvailable_Boolean.Value(false))
            .Build();

        // create a stub for the destination
        var destination = new ItemBuilder<IDestination>().Build();

        // act and assert

        // an exception should be thrown because the potion and the ingredients to brew the potion are not available.
        Assert.Throws<PotionNotAvailableException>(() =>
            deliveryService.Deliver(potionName, destination));
    }

    [Test]
    public void Package_when_potion_in_storage_available_will_be_send_to_destination()
    {
        // arrange

        // create a delivery service where the potion is available
        // and the potionRecipes is setuped to return unique value for ingredients, effect and color
        // the name is setuped to return MyPotion.
        var (deliveryService, scope) = new DeliveryServiceaa96Builder()
            .BuildWithScope();

        var destinationMock = new Mock<IDestination>();

        // the scope can be used to retrieve the unique values generated for the potionRecipes service.
        var expectedPotion = new Potion(
            scope.Get(p => p.Ctor.potionRecipes.GetPotionName_String),
            scope.Get(p => p.Ctor.potionRecipes.GetPotionRecipe_ValueTupleIIngredientIIngredient).Item1,
            scope.Get(p => p.Ctor.potionRecipes.GetPotionRecipe_ValueTupleIIngredientIIngredient).Item2,
            scope.Get(p => p.Ctor.potionRecipes.GetPotionEffect_IEffect),
            scope.Get(p => p.Ctor.potionRecipes.GetPotionColor_PotionColor));

        // act
        deliveryService.Deliver("MyPotion", destinationMock.Object);

        // assert

        // check that the destination has received a package which only contains the expectedPotion.
        destinationMock.Verify(destination => destination.Receive(It.Is<IPackage<IPotion>>(package => package.UnWrap().Single().Equals(expectedPotion))));
    }

    [Test]
    public void Package_when_potion_not_available_in_storage_a_brewed_will_be_send_to_destination()
    {
        // arrange
        var ingredients = new ItemBuilder<IIngredient>()
            .With(p => p.Name.Unique())
            .BuildMany(2);

        // setup the storage so only MyPotion is no available.
        var storage = Mock.Of<IStorage>(s =>
            s.CheckAvailable(It.IsAny<string>()) == true &&
            s.CheckAvailable("MyPotion") == false);

        // setup the delivery service. Use out storage mock, make use we use a cauldron stub so we can verify on it later.
        // Setup the PotionRecipes service to return the two ingredients created. 
        var (deliveryService, scope) = new ItemBuilder<DeliveryService>()
            .With(p => p.Ctor.storage.Value(storage))
            .With(p => p.Ctor.cauldron.Stub<ICauldron>())
            .With(p => p.Ctor.cauldron.Brew_IPotion.Stub<IPotion>())
            .With(p => p.Ctor.cauldron.Brew_IPotion.Name.Value("MyPotion"))
            .With(p => p.Ctor.potionRecipes.GetPotionRecipe_ValueTupleIIngredientIIngredient.Value((ingredients[0], ingredients[1])))
            .BuildWithScope();

        var destinationMock = new Mock<IDestination>();

        // act
        deliveryService.Deliver("MyPotion", destinationMock.Object);

        // assert

        // The send package should contain only out potion with the name MyPotion
        destinationMock.Verify(destination => destination.Receive(It.Is<IPackage<IPotion>>(package => package.UnWrap().Single().Name == "MyPotion")));

        // get the cauldron mock over the scope and verify that the potion was brewed.
        scope.GetAsMoq(p => p.Ctor.cauldron).Verify(cauldron => cauldron.Brew(ingredients[0], ingredients[1]));
    }
}