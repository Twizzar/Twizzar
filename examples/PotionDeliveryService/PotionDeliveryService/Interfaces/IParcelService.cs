namespace PotionDeliveryService.Interfaces;

public interface IParcelService
{
    void Send<T>(IPackage<T> package, IDestination destination);
}