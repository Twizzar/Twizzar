namespace PotionDeliveryService.Interfaces;

public interface IStorage
{
    void Store(IIngredient ingredient);

    IIngredient Take(string ingredientName);

    bool CheckAvailable(string ingredientName);
}