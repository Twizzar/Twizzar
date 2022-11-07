namespace PotionDeliveryService.Exceptions;

public class PotionNotAvailableException : Exception
{
    /// <inheritdoc />
    public PotionNotAvailableException(string potionName)
        : base($"Potion {potionName} is not available.")
    {
    }
}