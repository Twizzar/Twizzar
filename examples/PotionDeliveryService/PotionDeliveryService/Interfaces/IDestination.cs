namespace PotionDeliveryService.Interfaces;

public interface IDestination
{
    public void Receive<T>(IPackage<T> package);
}