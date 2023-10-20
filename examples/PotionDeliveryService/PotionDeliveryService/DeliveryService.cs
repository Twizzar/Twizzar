using PotionDeliveryService.Exceptions;
using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

/// <summary>
/// Service for delivering a potion.
/// This service checks if the potion is available in the storage,
/// when not it will brew a new one in the cauldron when the ingredients are available in the storage.
/// </summary>
public class DeliveryService : IDeliveryService
{
    private readonly IStorage _storage;
    private readonly ICauldron _cauldron;
    private readonly IPotionRecipes _potionRecipes;
    private readonly IParcelService _parcelService;
    private readonly IPackageFactory _packageFactory;

    public DeliveryService(
        IStorage storage,
        ICauldron cauldron,
        IPotionRecipes potionRecipes,
        IParcelService parcelService,
        IPackageFactory packageFactory)
    {
        this._storage = storage;
        this._cauldron = cauldron;
        this._potionRecipes = potionRecipes;
        this._parcelService = parcelService;
        this._packageFactory = packageFactory;
    }

    /// <summary>
    /// Deliver a potion by its name to a <see cref="IDestination"/>.
    /// </summary>
    /// <param name="potionName"></param>
    /// <param name="destination"></param>
    /// <exception cref="InvalidOperationException">The name is not a potion.</exception>
    /// <exception cref="PotionNotAvailableException">The potion is not available in the storage.</exception>
    public void Deliver(string potionName, IDestination destination)
    {
        if (this._storage.CheckAvailable(potionName))
        {
            var ingredient = this._storage.Take(potionName);

            if (ingredient is IPotion potion)
            {
                this.Send(potion, destination);
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
            this.Send(potion, destination);
        }
    }

    private void Send(IPotion potion, IDestination destination)
    {
        var package = this._packageFactory.CreatePackage(potion);
        package.Wrap();
        this._parcelService.Send(package, destination);
    }
}