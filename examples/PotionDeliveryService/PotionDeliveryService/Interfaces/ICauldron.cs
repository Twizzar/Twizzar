namespace PotionDeliveryService.Interfaces;

public interface ICauldron
{
    IPotion Brew(IIngredient ingredient1, IIngredient ingredient2);
}