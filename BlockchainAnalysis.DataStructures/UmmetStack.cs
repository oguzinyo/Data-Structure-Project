namespace BlockchainAnalysis.DataStructures;

public class UmmetStack<T>
{
    private T[] _items;
    private int _count;

    public UmmetStack(int capacity = 8)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));

        _items = new T[capacity];
    }

    public int Count => _count;
    public bool IsEmpty => _count == 0;

    public void Push(T item)
    {
        // Stack dolduğunda kapasiteyi dinamik olarak artırıyoruz
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
        
        // Garbage Collector'ın (GC) nesneyi bellekten atabilmesi için referansı kaldırıyoruz
        _items[_count] = default!;
        
        return item;
    }

    private void Resize(int newCapacity)
    {
        var resized = new T[newCapacity];

        // Mevcut elemanları yeni ve daha büyük diziye aktarıyoruz
        for (int i = 0; i < _count; i++)
        {
            resized[i] = _items[i];
        }

        _items = resized;
    }
}