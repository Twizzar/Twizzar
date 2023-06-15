namespace PotionDeliveryService.Exceptions;

public class PotionNotAvailableException : Exception
{
    public PotionNotAvailableException(string potionName)
        : base($"Potion {potionName} is not available.")
    {
    }
}