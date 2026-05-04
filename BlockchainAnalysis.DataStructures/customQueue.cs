namespace BlockchainAnalysis.DataStructures;

public class CustomQueue<T>
{
    private T[] _items;
    private int _head;
    private int _tail;
    private int _count;

    public CustomQueue(int capacity = 8)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));

        _items = new T[capacity];
    }

    public int Count => _count;
    public bool IsEmpty => _count == 0;

    public void Enqueue(T item)
    {
        if (_count == _items.Length)
        {
            Resize(_items.Length * 2);
        }

        _items[_tail] = item;
        _tail = (_tail + 1) % _items.Length;
        _count++;
    }

    public T Dequeue()
    {
        if (IsEmpty) throw new InvalidOperationException("Queue is empty.");

        var item = _items[_head];
        _items[_head] = default!;
        _head = (_head + 1) % _items.Length;
        _count--;
        return item;
    }

    private void Resize(int newCapacity)
    {
        var resized = new T[newCapacity];

        for (int i = 0; i < _count; i++)
        {
            resized[i] = _items[(_head + i) % _items.Length];
        }

        _items = resized;
        _head = 0;
        _tail = _count;
    }
}