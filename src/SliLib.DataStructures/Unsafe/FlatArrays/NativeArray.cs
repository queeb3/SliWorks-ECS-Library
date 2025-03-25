namespace SliLib.DataStructures;

internal unsafe struct NativeArray // allow for storage ambigously in another array
{
    private byte* buffer;
    private IndexEncoder encoder;
    private int length;
    private int elementSize;
    private int size;

    public int Count => encoder.PopCount();
    public int Capacity => length;

    public NativeArray(int capacity, int size)
    {
        elementSize = size;
        length = capacity;
        this.size = length * elementSize;

        encoder = new(length);

        buffer = (T*)Marshal.AllocHGlobal(this.size);
        Unsafe.InitBlock(buffer, 0, (uint)size);
    }


    public int Add<T>(T item) where T : unmanaged
    {
        int index = encoder.FirstFreeBit();
        if(index == -1) return index;

        encoder.Set(index);
        buffer[index * elementSize] = item;
    }

    public bool Remove(int index)
    {
        if (encoder.IsOccupied(index))
        {
            Unsafe.InitBlock(*buffer[index], 0, elementSize);
            encoder.Unset(index);
            return true;
        }

        return false;
    }

    public ref T Get<T>(int index) where T : unmanaged
    {
        if (!encoder.IsOccupied(index))
        {
            throw new InvalidOperationException($"No item was stored at index {index}.");
        }

        return ref *(T*)buffer[index * elementSize];
    }

    public bool Contains<T>(T item) where T : unmanaged
    {
        for(int i = 0; i < length; i++)
        {
            if(!encoder.IsOccupied(i)) continue;

            bool result = Externals.Compare<T>(item, (T)buffer[i * elementSize]);
            if (result) return true;
        }

        return false;
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

internal unsafe struct NativeArray<T> where T : unmanaged
{
    private byte* buffer;
    private IndexEncoder encoder;
    private int length;
    private int elementSize;
    private int size;

    public int Count => encoder.PopCount();
    public int Capacity => length;

    public NativeArray(int capacity, int size)
    {
        elementSize = size;
        length = capacity;
        this.size = length * elementSize;

        encoder = new(length);

        buffer = (byte*)Marshal.AllocHGlobal(this.size);
        Unsafe.InitBlock(buffer, 0, (uint)size);
    }

    public int Add(T item)
    {
        int index = encoder.FirstFreeBit();
        if(index == -1) return index;

        encoder.Set(index);
        *(T*)(buffer + index * elementSize]) = item;
    }

    public bool Remove(int index)
    {
        if (encoder.IsOccupied(index))
        {
            Unsafe.InitBlock(buffer + index * elementSize, 0, (uint)elementSize);
            encoder.Unset(index);
            return true;
        }

        return false;
    }

    public ref T Get(int index)
    {
        if (!encoder.IsOccupied(index))
        {
            throw new InvalidOperationException($"No item was stored at index {index}.");
        }

        return ref *(T*)buffer[index * elementSize];
    }

    public bool Contains(T item)
    {
        for(int i = 0; i < length; i++)
        {
            if (!encoder.IsOccupied(i)) continue;
            if (Externals.Compare(item, *(T*)(buffer + i * elementSize))) return true;
        }

        return false;
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
