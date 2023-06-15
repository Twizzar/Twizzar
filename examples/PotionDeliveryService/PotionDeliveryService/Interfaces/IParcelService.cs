namespace PotionDeliveryService.Interfaces;

/// <summary>
/// Service for sending out <see cref="IPackage{T}"/>.
/// </summary>
public interface IParcelService
{
    void Send(IPackage<IPotion> package, IDestination destination);
}