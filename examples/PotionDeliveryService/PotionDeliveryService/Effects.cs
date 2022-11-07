using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

public record Effect(string Name) : IEffect;

public static class Effects
{
    public static IEffect HealingEffect => new Effect("Healing Effect");
    public static IEffect ManaRestoreEffect => new Effect("Mana Restore Effect");
    public static IEffect ManaAndHealthRestoreEffect => new Effect("Mana and Health Restore Effect");
}