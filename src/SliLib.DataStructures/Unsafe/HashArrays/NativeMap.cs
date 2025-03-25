namespace SliLib.DataStructures;

internal unsafe struct MapNode<T> where T : unmanaged
{
    private T* buffer;
    private IndexEncoder encoder;

    private int size;
    private int length;
    private int elementSize;

    [SkipLocalsInit]
    public MapNode(int initialCapacity)
    {
        elementSize = sizeof(T);
        length = initialCapacity;
        size = length * elementSize;

        encoder = new(length);

        buffer = (T*)Marshal.AllocHGlobal(size);
        Unsafe.InitBlock((byte*)buffer, 0, (uint)size);
    }

    public void Add()
    {

    }

    public void Remove()
    {

    }

    public void Get()
    {

    }

    public void Contains()
    {

    }

    private void Expand()
    {
        int newLength = length * 2;
        int newSize = size * 2;
        T* destination = (T*)Marshal.AllocHGlobal(newSize);

        Unsafe.CopyBlock(destination, buffer, (uint)size);
        Unsafe.InitBlock(destination + length, 0, (uint)newSize);

        Marshal.FreeHGlobal((nint)buffer);
        encoder.Expand(newLength);
        buffer = destination;
        size = newSize;
        length = newLength;
    }

    public void Free()
    {
        if (buffer is not null)
        {
            Marshal.FreeHGlobal((nint)buffer);
            buffer = null;
        }
    }
}

public unsafe struct NativeMap<TKey, TValue> where TKey : unmanaged where TValue : unmanaged
{
    private MapNode<TValue>* buckets;
    private IndexEncoder allocated;

    private int size;
    private int length;
    private int count;

    public NativeMap(int initialCapacity)
    {
        length = initialCapacity;
        size = sizeof(MapNode<TValue>) * length;

        allocated = new(length);

        buckets = (MapNode<TValue>*)Marshal.AllocHGlobal(size);
        Unsafe.InitBlock((byte*)buckets, 0, (uint)size);
    }

    public void Add(TKey key, TValue value)
    {

    }

    public void Remove(TKey key)
    {

    }

    public ref TValue Get(TKey key)
    {

    }

    public bool ContainsKey(TKey key)
    {

    }

    public void Clear()
    {
        for (int i = 0; i < length; i++)
        {
            if (allocated.IsOccupied(i))
            {
                buckets[i].Free();
                allocated.Unset(i);
            }
        }
    }

    private int GetHash(TKey key)
    {
        int hash = -1;
        byte* raw = (byte*)&key;

        for (int i = 0; i < sizeof(TKey); i++)
        {
            hash = (((hash + 5) * 31) + raw[i]) * 7;
        }

        return hash & 0x7FFFFFFF;
    }

    private int HashIndex(int hash)
    {
        return hash % length;
    }

    private void NewBucket(int index)
    {
        buckets[index] = new(4);
    }

    private void Expand()
    {
        int newLength = length * 2;
        int newSize = size * 2;
        MapNode<TValue>* destination = (MapNode<TValue>*)Marshal.AllocHGlobal(newSize);

        // TODO not going to copy blocks but need store all values in a separate array for rehashing
        Unsafe.CopyBlock(destination, buckets, (uint)size);
        Unsafe.InitBlock(destination + length, 0, (uint)(newSize-size));


        allocated.Free();
        allocated = new(newLength);

        // TODO rehash all stored


        Marshal.FreeHGlobal((nint)buckets);
        buckets = destination;
        length = newLength;
        size = newSize;
    }

    public void Free()
    {
        for (int i = 0; i < length; i++)
            buckets[i].Free();

        Marshal.FreeHGlobal((nint)buckets);
        buckets = null;
    }
}
