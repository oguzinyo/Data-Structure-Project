namespace BlockchainAnalysis.DataStructures;

public class CustomStack<T>
{
    private T[] _items;
    private int _count;

    public CustomStack(int capacity = 8)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));

        _items = new T[capacity];
    }

    public int Count => _count;
    public bool IsEmpty => _count == 0;

    public void Push(T item)
    {
        if (_count == _items.Length)
        {
            Resize(_items.Length * 2);
        }

        _items[_count] = item;
        _count++;
    }

    public T Pop()
    {
        if (IsEmpty) throw new InvalidOperationException("Stack is empty.");

        _count--;
        var item = _items[_count];
        _items[_count] = default!;
        return item;
    }

    private void Resize(int newCapacity)
    {
        var resized = new T[newCapacity];

        for (int i = 0; i < _count; i++)
        {
            resized[i] = _items[i];
        }

        _items = resized;
    }
}
