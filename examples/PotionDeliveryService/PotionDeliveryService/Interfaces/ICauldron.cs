namespace PotionDeliveryService.Interfaces;

/// <summary>
/// Brews <see cref="IPotion"/> out of <see cref="IIngredient"/>s.
/// </summary>
public interface ICauldron
{
    IPotion Brew(IIngredient ingredient1, IIngredient ingredient2);
}