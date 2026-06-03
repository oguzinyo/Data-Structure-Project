namespace BlockchainAnalysis.DataStructures;

public class UmmetQueue<T>
{
    private T[] _items;
    private int _head;
    private int _tail;
    private int _count;

    public UmmetQueue(int capacity = 8)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));

        _items = new T[capacity];
    }

    public int Count => _count;
    public bool IsEmpty => _count == 0;

    public void Enqueue(T item)
    {
        // Dizi kapasitesi dolduğunda boyutu iki katına çıkarıyoruz (Dinamik büyüme)
        if (_count == _items.Length)
        {
            Resize(_items.Length * 2);
        }

        _items[_tail] = item;
        // Dairesel (circular) kuyruk yapısı için modüler aritmetik kullanıyoruz
        _tail = (_tail + 1) % _items.Length;
        _count++;
    }

    public T Dequeue()
    {
        if (IsEmpty) throw new InvalidOperationException("Queue is empty.");

        var item = _items[_head];
        
        // Bellek sızıntısını (memory leak) önlemek için referansı temizliyoruz
        _items[_head] = default!;
        
        // Head işaretçisini dairesel olarak bir ileri kaydırıyoruz
        _head = (_head + 1) % _items.Length;
        _count--;
        
        return item;
    }

    private void Resize(int newCapacity)
    {
        var resized = new T[newCapacity];

        // Dairesel kuyruktaki elemanları yeni diziye düz bir şekilde (0. indeksten başlayarak) kopyalıyoruz
        for (int i = 0; i < _count; i++)
        {
            resized[i] = _items[(_head + i) % _items.Length];
        }

        _items = resized;
        _head = 0;
        _tail = _count;
    }
}