using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

public static class PotionColorExtensions
{
    public static ConsoleColor GetConsoleColor(this PotionColor potionColor) =>
        potionColor switch
        {
            PotionColor.Blue => ConsoleColor.Blue,
            PotionColor.Red => ConsoleColor.Red,
            PotionColor.Purple => ConsoleColor.DarkMagenta,
            _ => throw new NotImplementedException(),
        };
}