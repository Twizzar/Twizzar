using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

/// <summary>
/// Service for brewing potions.
/// </summary>
public class Cauldron : ICauldron
{
    private readonly IPotionRecipes _recipes;

    public Cauldron(IPotionRecipes recipes)
    {
        this._recipes = recipes;
    }

    public IPotion Brew(IIngredient ingredient1, IIngredient ingredient2)
    {
        var potionName = this._recipes.GetPotionName(ingredient1, ingredient2);

        PrintCauldron(ingredient1.Name, ingredient2.Name);

        var effect = this._recipes.GetPotionEffect(potionName);
        var color = this._recipes.GetPotionColor(potionName);
        var potion = new Potion(potionName, ingredient1, ingredient2, effect, color);

        PrintPotion(potion);
        return potion;
    }

    private static void PrintCauldron(string ingredient1, string ingredient2)
    {
        Console.WriteLine($@"
{ingredient1} + {ingredient2}
              (
               )  )
           ______(____
          (___________)
           /         \
          /           \
         |             |
     ____\             /____
    ()____'.__     __.'____()
         .'` .'```'. `-.
        ().'`       `'.()
");
    }

    private static void PrintPotion(IPotion potion)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = potion.Color.GetConsoleColor();
        Console.WriteLine(@$"
   |█████|
   |_   _|
    |   |
    |   |
   /     \
  /       \
 /░░░░░░░░░\
|░░░░░░░░░░░|
|░░░░░░░░░░░|
 \░░░░░░░░░/

  {potion.Name}
  {potion?.Effect?.Name}
");

        Console.ForegroundColor = oldColor;
    }
}