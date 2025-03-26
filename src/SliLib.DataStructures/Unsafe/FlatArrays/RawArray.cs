namespace SliLib.DataStructures;

/// <summary>
/// Stores elements as type erased bytes requiring proper T casts in methods to get and store elements.
/// <br/>
/// NOTE: Purely internal and will not be exposed to public to prevent idiots from corrupting data.
/// <br/><br/>
/// DANGER: Do not insert different (typed/sized) items this can cause bad data integrity!
/// </summary>
internal unsafe struct RawArray
{
    private byte* buffer;
    private IndexEncoder encoder; // index tracker
    private int length;
    private int elementSize;
    private int size;

    /// <summary>
    /// Total size of the allocated buffer.
    /// </summary>
    public readonly int Size => size;
    public readonly int Count => encoder.PopCount();
    public readonly int Capacity => length;

    /// <summary>
    /// Creates a new heap allocated buffer that must be manually Free()'d.
    /// </summary>
    /// <param name="capacity">how many elements stored.</param>
    /// <param name="size">size of the element to be stored.</param>
    public RawArray(int capacity, int size)
    {
        elementSize = size;
        length = capacity;
        this.size = length * elementSize;

        encoder = new(length);

        buffer = (byte*)Marshal.AllocHGlobal(this.size);
        Unsafe.InitBlock(buffer, 0, (uint)size);
    }

    /// <summary>
    /// Attempts to add an item to a free index.
    /// </summary>
    /// <typeparam name="T">Cast</typeparam>
    /// <param name="item">Item to add</param>
    /// <returns>Index where it was placed, else -1 if no slots available.</returns>
    public int Add<T>(T item) where T : unmanaged
    {
        int index = encoder.FirstFreeBit();
        if (index == -1) return index;

        encoder.Set(index);
        *(T*)(buffer + index * elementSize) = item;
        return index;
    }

    /// <summary>
    /// Attempts to directly remove an item.
    /// <br/>
    /// NOTE: Zeros memory.
    /// </summary>
    /// <typeparam name="T">Cast</typeparam>
    /// <param name="item">item to compare</param>
    /// <returns>True if removed, else false.</returns>
    public bool Remove<T>(T item) where T : unmanaged
    {
        if (!TryGetIndex(item, out var index)) return false;

        if (!encoder.IsOccupied(index))
        {
            return false;
        }

        Unsafe.InitBlock(buffer + index * elementSize, 0, (uint)elementSize);
        encoder.Unset(index);
        return true;
    }

    /// <summary>
    /// Gets the exact item in memory for in place transforms.
    /// </summary>
    /// <typeparam name="T">Cast</typeparam>
    /// <param name="index">Item locatation</param>
    /// <returns>exact reference to to stored item in memory.</returns>
    /// <exception cref="InvalidOperationException">Ensure the index has an item.</exception>
    public ref T Get<T>(int index) where T : unmanaged
    {
        if (!encoder.IsOccupied(index))
        {
            throw new InvalidOperationException($"No item was stored at index {index}.");
        }

        return ref *(T*)(buffer + index * elementSize);
    }

    internal ref T RawGet<T>(int index) where T : unmanaged
    {
        if (index < 0 || index >= length)
        {
            throw new IndexOutOfRangeException();
        }

        return ref *(T*)(buffer + index * elementSize);
    }

    /// <summary>
    /// Checks if an item is stored in the array.
    /// </summary>
    /// <typeparam name="T">Cast</typeparam>
    /// <param name="item">Item to compare</param>
    /// <returns>True if found, else False.</returns>
    public bool Contains<T>(T item) where T : unmanaged
    {
        for (int i = 0; i < length; i++)
        {
            if (!encoder.IsOccupied(i)) continue;

            bool result = Externals.Compare(&item, (T*)(buffer + i * elementSize));
            if (result) return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to get the index of a item should it exist in the array.
    /// <br/>
    /// Note: Acts exactly like Contains().
    /// </summary>
    /// <typeparam name="T">Cast</typeparam>
    /// <param name="item">Item to compare</param>
    /// <param name="index">Where its stored</param>
    /// <returns>True if found, else false and index = -1.</returns>
    public bool TryGetIndex<T>(T item, out int index) where T : unmanaged
    {
        for (int i = 0; i < length; i++)
        {
            if (!encoder.IsOccupied(i)) continue;

            bool result = Externals.Compare(&item, (T*)(buffer + i * elementSize));
            if (result) { index = i; return true; }
        }

        index = -1;
        return false;
    }

    /// <summary>
    /// Frees the buffer and the encoder from the heap.
    /// </summary>
    public void Free()
    {
        if (buffer is not null)
        {
            Marshal.FreeHGlobal((nint)buffer);
            buffer = null;
            encoder.Free();
        }
    }
}

public struct RawArray<T> : IDisposable where T : unmanaged
{
    private RawArray raw;
    public int Capacity => raw.Capacity;
    public int Count => raw.Count;
    public int Size => raw.Size;


    internal RawArray(RawArray raw)
    {
        this.raw = raw;
    }

    public RawArray(int capacity, int size)
    {
        raw = new(capacity, size);
    }

    /// <summary>
    /// Gets the exact item in memory for in place transforms.
    /// </summary>
    /// <param name="index">Item locatation</param>
    /// <returns>ref T to stored item in memory.</returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public ref T this[int index]
    {
        get => ref raw.RawGet<T>(index);
    }

    /// <summary>
    /// Attempts to add an item to a free index.
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <returns>Index where it was placed, else -1 if no slots available.</returns>
    public int Add(T item) => raw.Add(item);
    /// <summary>
    /// Attempts to directly remove an item.
    /// <br/>
    /// NOTE: Zeros memory.
    /// </summary>
    /// <param name="item">item to compare</param>
    /// <returns>True if removed, else false.</returns>
    public bool Remove(T item) => raw.Remove(item);
    /// <summary>
    /// Checks if an item is stored in the array.
    /// </summary>
    /// <param name="item">Item to compare</param>
    /// <returns>True if found, else False.</returns>
    public bool Contains(T item) => raw.Contains(item);
    /// <summary>
    /// Attempts to get the index of a item should it exist in the array.
    /// <br/>
    /// Note: Acts exactly like Contains().
    /// </summary>
    /// <param name="item">Item to compare</param>
    /// <param name="index">Where its stored</param>
    /// <returns>True if found, else false and index = -1.</returns>
    public bool TryGetIndex(T item, out int index) => raw.TryGetIndex(item, out index);
    /// <summary>
    /// Frees the buffer and the encoder from the heap.
    /// </summary>
    public void Free() => raw.Free();
    public void Dispose()
    {
        raw.Free();
        GC.SuppressFinalize(this);
    }

    public Span<T> AsSpan()
    {
        return MemoryMarshal.CreateSpan(ref this[0], raw.Capacity);
    }
}
