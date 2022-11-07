namespace PotionDeliveryService.Interfaces;

public interface IDeliveryService
{
    void Deliver(string potionName, IDestination destination);
}