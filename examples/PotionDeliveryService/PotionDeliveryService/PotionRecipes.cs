using System.Collections.Immutable;

using PotionDeliveryService.Exceptions;
using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

public class PotionRecipes : IPotionRecipes
{
    private readonly ImmutableDictionary<string, IPotion> _potions;
    private readonly ImmutableDictionary<(IIngredient, IIngredient), IPotion> _ingredientsToPotions;

    internal PotionRecipes(ImmutableDictionary<string, IPotion> potions, ImmutableDictionary<(IIngredient, IIngredient), IPotion> ingredientsToPotions)
    {
        this._potions = potions;
        this._ingredientsToPotions = ingredientsToPotions;
    }

    public static PotionRecipes Default => Create(Potions.HealthPotion, Potions.ManaPotion, Potions.RecoveryPotion);

    public static PotionRecipes Create(params IPotion[] potions) =>
        new(
            potions.ToImmutableDictionary(potion => potion.Name),
            potions.ToImmutableDictionary(potion => (potion.Ingredient1, potion.Ingredient2)));

    public PotionColor GetPotionColor(string potionName)
    {
        if (!this._potions.ContainsKey(potionName))
        {
            throw new PotionNotFoundException(potionName);
        }

        return this._potions[potionName].Color;
    }

    public IEffect GetPotionEffect(string potionName)
    {
        if (!this._potions.ContainsKey(potionName))
        {
            throw new PotionNotFoundException(potionName);
        }

        return this._potions[potionName].Effect;
    }

    public string GetPotionName(IIngredient ingredient1, IIngredient ingredient2)
    {
        if (!this._ingredientsToPotions.ContainsKey((ingredient1, ingredient2)))
        {
            throw new RecipeNotFoundException(ingredient1, ingredient2);
        }

        return this._ingredientsToPotions[(ingredient1, ingredient2)].Name;
    }

    public (IIngredient ingredient1, IIngredient ingredient2) GetPotionRecipe(string potionName)
    {
        if (!this._potions.ContainsKey(potionName))
        {
            throw new PotionNotFoundException(potionName);
        }

        return (this._potions[potionName].Ingredient1, this._potions[potionName].Ingredient2);
    }
}