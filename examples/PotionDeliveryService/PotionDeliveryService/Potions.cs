using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

public record Potion(string Name, IIngredient Ingredient1, IIngredient Ingredient2, IEffect Effect, PotionColor Color) : IPotion;

public static class Potions
{
    public static IPotion HealthPotion =>
        new Potion("Health Potion", Ingredients.Water, Ingredients.RedBerry, Effects.HealingEffect, PotionColor.Red);

    public static IPotion ManaPotion =>
        new Potion("Mana Potion", Ingredients.Water, Ingredients.GlowingMushroom, Effects.ManaRestoreEffect, PotionColor.Blue);

    public static IPotion RecoveryPotion =>
        new Potion("Recovery Potion", HealthPotion, ManaPotion, Effects.ManaAndHealthRestoreEffect, PotionColor.Purple);
}