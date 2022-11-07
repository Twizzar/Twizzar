namespace PotionDeliveryService.Exceptions;

public class PotionNotFoundException : Exception
{
    /// <inheritdoc />
    public PotionNotFoundException(string potionName)
        : base($"{potionName} not found.")
    {
    }
}