using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

public record Potion(
    string Name,
    IIngredient Ingredient1,
    IIngredient Ingredient2,
    IEffect Effect,
    PotionColor Color) : IPotion
{
    /// <summary>
    /// Constructor for building a Potion where the name is the color + Potion.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="ingredient1"></param>
    /// <param name="ingredient2"></param>
    /// <param name="effect"></param>
    public Potion(PotionColor color, IIngredient ingredient1, IIngredient ingredient2, IEffect effect)
        : this($"{color}Potion", ingredient1, ingredient2, effect, color)
    {
    }

    public double Price { get; set; }
}

public static class Potions
{
    public static IPotion HealthPotion =>
        new Potion("Health Potion", Ingredients.Water, Ingredients.RedBerry, Effects.HealingEffect, PotionColor.Red);

    public static IPotion ManaPotion =>
        new Potion("Mana Potion", Ingredients.Water, Ingredients.GlowingMushroom, Effects.ManaRestoreEffect, PotionColor.Blue);

    public static IPotion RecoveryPotion =>
        new Potion("Recovery Potion", HealthPotion, ManaPotion, Effects.ManaAndHealthRestoreEffect, PotionColor.Purple);
}