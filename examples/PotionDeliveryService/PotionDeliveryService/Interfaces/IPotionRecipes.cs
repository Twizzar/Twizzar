using PotionDeliveryService.Exceptions;

namespace PotionDeliveryService.Interfaces
{
    public interface IPotionRecipes
    {
        /// <summary>
        /// Get the recipe for a certain potion.
        /// </summary>
        /// <param name="potionName"></param>
        /// <exception cref="RecipeNotFoundException">When potion recipe was not found.</exception>
        /// <returns>Tuple with the two ingredients needed for the potion.</returns>
        (IIngredient ingredient1, IIngredient ingredient2) GetPotionRecipe(string potionName);

        /// <summary>
        /// Get the potion name from the ingredients.
        /// </summary>
        /// <param name="ingredient1"></param>
        /// <param name="ingredient2"></param>
        /// <exception cref="RecipeNotFoundException">When potion recipe was not found.</exception>
        /// <returns>The potion name.</returns>
        string GetPotionName(IIngredient ingredient1, IIngredient ingredient2);

        IEffect GetPotionEffect(string potionName);

        PotionColor GetPotionColor(string potionName);
    }
}