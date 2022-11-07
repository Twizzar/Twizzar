using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests;

[TestFixture]
public partial class CauldronTests
{
    [Test]
    public void Potion_gets_brewed_correctly()
    {
        // arrange
        var ingredients = new ItemBuilder<IIngredient>()
            .With(p => p.Name.Value("Test Ingredient"))
            .BuildMany(2);

        var (cauldron, scope) = new TestCaldronBuilder()
            .BuildWithScope();

        // act
        var potion = cauldron.Brew(ingredients[0], ingredients[1]);
        
        // assert
        Assert.Multiple(() =>
        {
            Assert.That(potion.Color, Is.EqualTo(scope.Get(p => p.Ctor.recipes.GetPotionColor_PotionColor)));
            Assert.That(potion.Name, Is.EqualTo(scope.Get(p => p.Ctor.recipes.GetPotionName_String)));
        });
    }
}