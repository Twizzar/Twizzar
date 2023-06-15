using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Exceptions;

public class RecipeNotFoundException : Exception
{
    public RecipeNotFoundException(IIngredient ingredient1, IIngredient ingredient2)
        : base($"Recipe for the ingredients {ingredient1.Name} and {ingredient2.Name} not found.")
    {
    }
}