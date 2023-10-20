using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

public class Storage : IStorage
{
    private readonly Dictionary<string, Stock> _stocks = new();

    /// <inheritdoc />
    public void Store(IIngredient ingredient)
    {
        if (this._stocks.TryGetValue(ingredient.Name, out var stock))
        {
            stock.Amount++;
        }
        else
        {
            this._stocks.Add(ingredient.Name, new Stock(ingredient));
        }
    }

    /// <inheritdoc />
    public IIngredient Take(string ingredientName)
    {
        if (!this._stocks.ContainsKey(ingredientName))
        {
            throw new ArgumentException($"Ingredient with name {ingredientName} does not exist in storage.");
        }

        var stock = this._stocks[ingredientName];

        if (stock.Amount == 0)
        {
            throw new ArgumentException($"Ingredient with name {ingredientName} is out of stock.");
        }

        stock.Amount--;
        return stock.Ingredient;
    }

    /// <inheritdoc />
    public bool CheckAvailable(string ingredientName) =>
        this._stocks.ContainsKey(ingredientName) && this._stocks[ingredientName].Amount > 0;

    private class Stock
    {
        public Stock(IIngredient ingredient)
        {
            this.Ingredient = ingredient;
            this.Amount = 1;
        }

        public int Amount { get; set; }

        public IIngredient Ingredient { get; init; }
    }
}