namespace PotionDeliveryService.Tests;

[TestFixture]
public partial class CauldronTests
{
    [Test]
    [TestSource(nameof(Cauldron.Brew))]
    public void Potion_gets_brewed_correctly()
    {
        // arrange

        // For brewing two ingredients are needed. Two are created with a unique name.
        var ingredients = new ItemBuilder<IIngredient>()
            .With(p => p.Name.Unique())
            .BuildMany(2);

        // setup the cauldron:
        // the potionRecipes service should return a name, an effect and a color when asked.
        var cauldron = new TestCauldronBuilder()
            .Build(out var scope);

        // act
        var potion = cauldron.Brew(ingredients[0], ingredients[1]);

        // assert

        // check that the ingredients where used.
        Assert.That(potion.Ingredient1, Is.EqualTo(ingredients[0]));
        Assert.That(potion.Ingredient2, Is.EqualTo(ingredients[1]));

        // check that the returned values form the potionRecipes where used for creating the potion
        Assert.That(potion.Color, Is.EqualTo(scope.Get(p => p.Ctor.recipes.GetPotionColor)));
        Assert.That(potion.Name, Is.EqualTo(scope.Get(p => p.Ctor.recipes.GetPotionName)));
        Assert.That(potion.Effect, Is.EqualTo(scope.Get(p => p.Ctor.recipes.GetPotionEffect)));
    }
}