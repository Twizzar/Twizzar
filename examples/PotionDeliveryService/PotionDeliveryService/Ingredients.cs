using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

public record Ingredient(string Name) : IIngredient;

public static class Ingredients
{
    public static IIngredient Tumbleweed => new Ingredient("Tumbleweed");
    public static IIngredient Skull => new Ingredient("Skull");
    public static IIngredient GlowingMushroom => new Ingredient("Glowing Mushroom");
    public static IIngredient Water => new Ingredient("Water");
    public static IIngredient RedBerry => new Ingredient("RedBerry");
}