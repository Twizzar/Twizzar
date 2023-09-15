namespace PotionDeliveryService.Tests;

[TestFixture]
public partial class PotionRecipesTests
{
    [Test]
    [TestSource(nameof(PotionRecipes.GetPotionRecipe))]
    public void When_name_not_in_the_dictionary_GetPotionRecipe_throws_PotionNotFoundException()
    {
        // arrange
        var potionRecipes = new EmptyPotionRecipesBuilder()
            .Build();

        // act & assert
        Assert.Throws<PotionNotFoundException>(() => potionRecipes.GetPotionRecipe(string.Empty));
    }

    [Test]
    [TestSource(nameof(PotionRecipes.GetPotionEffect))]
    public void When_name_not_in_the_dictionary_GetPotionEffect_throws_PotionNotFoundException()
    {
        // arrange
        var potionRecipes = new EmptyPotionRecipesBuilder()
            .Build();

        // act & assert
        Assert.Throws<PotionNotFoundException>(() => potionRecipes.GetPotionEffect(string.Empty));
    }

    [Test]
    [TestSource(nameof(PotionRecipes.GetPotionColor))]
    public void When_name_not_in_the_dictionary_GetPotionColor_throws_PotionNotFoundException()
    {
        // arrange
        var potionRecipes = new EmptyPotionRecipesBuilder()
            .Build();

        // act & assert
        Assert.Throws<PotionNotFoundException>(() => potionRecipes.GetPotionColor(string.Empty));
    }

    [Test]
    [TestSource(nameof(PotionRecipes.GetPotionName))]
    public void When_name_not_in_the_dictionary_GetPotionName_throws_RecipeNotFoundException()
    {
        // arrange
        var ingredients = new ItemBuilder<IIngredient>()
            .BuildMany(2);

        var potionRecipes = new EmptyPotionRecipesBuilder()
            .Build();

        // act & assert
        Assert.Throws<RecipeNotFoundException>(() => potionRecipes.GetPotionName(ingredients[0], ingredients[1]));
    }
}