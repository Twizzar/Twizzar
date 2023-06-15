namespace PotionDeliveryService.Interfaces;

/// <summary>
/// Stores <see cref="IIngredient"/> which also can be take out of the storage.
/// </summary>
public interface IStorage
{
    void Store(IIngredient ingredient);

    IIngredient Take(string ingredientName);

    bool CheckAvailable(string ingredientName);
}