using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService;

public enum PackageState
{
    Open,
    Wrapped,
    UnWrapped,
}

public class Package<T> : IPackage<T>
{
    private readonly List<T> _items;
    private PackageState _state = PackageState.Open;

    public Package()
    {
        this._items = new List<T>();
    }

    public Package(params T[] items)
    {
        this._items = items.ToList();
    }

    public void Add(T item)
    {
        this.GuardIsOpen();

        this._items.Add(item);
    }

    public IEnumerable<T> UnWrap()
    {
        if (this._state != PackageState.Wrapped)
        {
            throw new InvalidOperationException("Packages is not wrapped");
        }

        this._state = PackageState.UnWrapped;
        return this._items;
    }

    public void Wrap()
    {
        this.GuardIsOpen();

        this._state = PackageState.Wrapped;
    }

    private void GuardIsOpen()
    {
        if (this._state != PackageState.Open)
        {
            throw new InvalidOperationException("Package is already wrapped.");
        }
    }
}