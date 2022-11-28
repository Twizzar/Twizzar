using PotionDeliveryService.Exceptions;
using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

public class DeliveryService : IDeliveryService
{
    private readonly IStorage _storage;
    private readonly ICauldron _cauldron;
    private readonly IPotionRecipes _potionRecipes;

    public DeliveryService(IStorage storage, ICauldron cauldron, IPotionRecipes potionRecipes)
    {
        this._storage = storage;
        this._cauldron = cauldron;
        this._potionRecipes = potionRecipes;
    }

    /// <summary>
    /// Deliver a potion by its name to a <see cref="IDestination"/>.
    /// </summary>
    /// <param name="potionName"></param>
    /// <param name="destination"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="PotionNotAvailableException"></exception>
    public void Deliver(string potionName, IDestination destination)
    {
        if (this._storage.CheckAvailable(potionName))
        {
            var ingredient = this._storage.Take(potionName);

            if (ingredient is IPotion potion)
            {
                Send(potion, destination);
            }
            else
            {
                throw new InvalidOperationException($"The name {potionName} is not an Potion.");
            }
        }

        // Potion not available in storage
        else
        {
            var (ingredient1, ingredient2) = this._potionRecipes.GetPotionRecipe(potionName);

            if (!this._storage.CheckAvailable(ingredient1.Name) || !this._storage.CheckAvailable(ingredient2.Name))
            {
                throw new PotionNotAvailableException(potionName);
            }

            var potion = this._cauldron.Brew(ingredient1, ingredient2);
            Send(potion, destination);
        }
    }

    private static void Send(IPotion potion, IDestination destination)
    {
        var package = new Package<IPotion>(potion);
        package.Wrap();
        destination.Receive(package);
    }
}