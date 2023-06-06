namespace PotionDeliveryService.Interfaces;

public interface IPotion : IIngredient
{
    IIngredient Ingredient1 { get; }

    IIngredient Ingredient2 { get; }

    IEffect Effect { get; }

    PotionColor Color { get; }
}