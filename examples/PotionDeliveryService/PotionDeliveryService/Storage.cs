using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

public class Storage : IStorage
{
    private readonly Dictionary<string, Queue<IIngredient>> _storedIngredients = new();

    /// <inheritdoc />
    public void Store(IIngredient ingredient)
    {
        if (this._storedIngredients.TryGetValue(ingredient.Name, out var storedIngredient))
        {
            storedIngredient.Enqueue(ingredient);
        }
        else
        {
            this._storedIngredients.Add(ingredient.Name, new Queue<IIngredient>());
            this._storedIngredients[ingredient.Name].Enqueue(ingredient);
        }
    }

    /// <inheritdoc />
    public IIngredient Take(string ingredientName)
    {
        if (!this._storedIngredients.ContainsKey(ingredientName) || this._storedIngredients[ingredientName].Count <= 0)
        {
            throw new InvalidOperationException($"Cannot take out {ingredientName} because none is stored");
        }

        return this._storedIngredients[ingredientName].Dequeue();
    }

    /// <inheritdoc />
    public bool CheckAvailable(string ingredientName) =>
        this._storedIngredients.ContainsKey(ingredientName) && this._storedIngredients[ingredientName].Count > 0;
}