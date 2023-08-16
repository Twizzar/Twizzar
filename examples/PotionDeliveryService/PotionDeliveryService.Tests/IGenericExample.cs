namespace PotionDeliveryService.Tests;

public interface IGenericExample
{
    int MyMethod();

    T MyMethod<T>();

    (TItem1 a, TItem2 b) MyMethod<TItem1, TItem2>();

    T SimpleGenericMethod<T>();

    IList<T> CreateList<T>(params T[] items);

    T StructConstrain<T>(T parameter)
        where T : struct;

    T ClassConstrain<T>(T parameter)
        where T : class;

    T InterfaceConstrain<T>(T parameter)
        where T : IIngredient;

    T MultiConstrains<T>()
        where T : IPotion, IIngredient;
}