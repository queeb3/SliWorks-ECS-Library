namespace SliLib.DataStructures;

public unsafe class PaddedArray<T> where T : unmanaged
{
    private PaddedArray array;
    public int Capacity => array.Length;
    public int Count { get; private set; }

    public PaddedArray(int capacity)
    {
        array = new PaddedArray(capacity, sizeof(T));
    }

    public int Add(T data)
    {
        Count++;
        return array.Add(data);
    }

    public bool Remove(T item)
    {
        bool removed = array.Remove(item, out _);
        if (removed) Count--;
        return removed;
    }

    public bool Contains(T item)
    {
        return array.Contains(item, out _);
    }

    public bool TryGetIndex(T item, out int index)
    {
        return array.Contains(item, out index);
    }

    public void Dispose()
    {
        array.Free();  // Manually free unmanaged memory
        GC.SuppressFinalize(this); // Prevents the finalizer from running (no need)
    }

    ~PaddedArray() => array.Free();
}
