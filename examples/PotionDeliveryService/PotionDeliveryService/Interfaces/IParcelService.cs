namespace PotionDeliveryService.Interfaces;

public interface IParcelService
{
    void Send(IPackage<IPotion> package, IDestination destination);
}