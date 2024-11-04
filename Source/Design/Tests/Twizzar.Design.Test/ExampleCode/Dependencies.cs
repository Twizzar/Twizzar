using System;
using System.Collections;

#pragma warning disable IDE1006 // Naming Styles

namespace ExampleCode;

public interface InterfaceA
{

}

public class ImplementationA : InterfaceA
{

}

public class ImplementationB : InterfaceA
{

}

public class ExtensionA : ImplementationA
{

}

public class ExtensionB : ImplementationA
{

}

public class MyList : IList
{
    #region Implementation of IEnumerable

    /// <inheritdoc />
    public IEnumerator GetEnumerator() =>
        throw new NotImplementedException();

    #endregion

    #region Implementation of ICollection

    /// <inheritdoc />
    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public int Count { get; }

    /// <inheritdoc />
    public object SyncRoot { get; }

    /// <inheritdoc />
    public bool IsSynchronized { get; }

    #endregion

    #region Implementation of IList

    /// <inheritdoc />
    public int Add(object value) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public bool Contains(object value) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void Clear()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public int IndexOf(object value) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void Insert(int index, object value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Remove(object value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public object this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool IsReadOnly { get; }

    /// <inheritdoc />
    public bool IsFixedSize { get; }

    #endregion
}