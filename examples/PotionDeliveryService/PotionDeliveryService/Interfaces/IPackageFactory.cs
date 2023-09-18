namespace PotionDeliveryService.Interfaces;

public interface IPackageFactory
{
    IPackage<T> CreatePackage<T>(params T[] items);
}