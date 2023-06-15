namespace PotionDeliveryService.Interfaces;

public interface IPackage<T>
{
    void Add(T item);

    void Wrap();

    IEnumerable<T> UnWrap();
}