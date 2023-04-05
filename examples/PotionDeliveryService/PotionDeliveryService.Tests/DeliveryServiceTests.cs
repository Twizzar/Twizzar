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
            .With(p => p.Ctor.storage.CheckAvailable.Value(true))
            .With(p => p.Ctor.storage.Take.Stub<IIngredient>())
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
            .With(p => p.Ctor.storage.CheckAvailable.Value(false))
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
        var deliveryService= new DeliveryServiceaa96Builder()
            .Build(out var scope);

        var destinationMock = new Mock<IDestination>();

        // the scope can be used to retrieve the unique values generated for the potionRecipes service.
        var expectedPotionName = scope.Get(p => p.Ctor.potionRecipes.GetPotionName);

        // act
        deliveryService.Deliver("MyPotion", destinationMock.Object);

        // assert

        // check that the destination has received a package which only contains the expectedPotion.
        destinationMock.Verify(destination => destination.Receive(It.Is<IPackage<IPotion>>(package => package.UnWrap().Single().Name.Equals(expectedPotionName))));
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
        var deliveryService = new DeliveryServiceBuilder()
            .With(p => p.Ctor.storage.Value(storage))
            .With(p => p.Ctor.cauldron.Stub<ICauldron>())
            .With(p => p.Ctor.cauldron.Brew.Stub<IPotion>())
            .With(p => p.Ctor.cauldron.Brew.Name.Value("MyPotion"))
            .With(p => p.Ctor.potionRecipes.GetPotionRecipe.Value((ingredients[0], ingredients[1])))
            .Build(out var scope);

        var destinationMock = new Mock<IDestination>();

        // act
        deliveryService.Deliver("MyPotion", destinationMock.Object);

        // assert

        // The send package should contain only out potion with the name MyPotion
        destinationMock.Verify(destination => destination.Receive(It.Is<IPackage<IPotion>>(package => package.UnWrap().Single().Name == "MyPotion")));

        // get the cauldron mock over the scope and verify that the potion was brewed.
        scope.Verify(p => p.Ctor.cauldron.Brew)
            .WhereIngredient1Is(ingredients[0])
            .WhereIngredient2Is(ingredients[1])
            .Called();
    }
}