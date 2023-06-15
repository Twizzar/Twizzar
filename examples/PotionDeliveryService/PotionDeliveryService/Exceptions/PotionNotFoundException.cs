namespace PotionDeliveryService.Exceptions;

public class PotionNotFoundException : Exception
{
    public PotionNotFoundException(string potionName)
        : base($"{potionName} not found.")
    {
    }
}