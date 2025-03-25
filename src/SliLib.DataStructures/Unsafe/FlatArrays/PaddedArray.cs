namespace SliLib.DataStructures;

internal unsafe struct PaddedArray
{
    private byte* buffer;
    private IndexEncoder encoder;
    private int size;
    private int elementSize;
    private int length; // also acts as max bit for encoder

    private int indexOffset;
    private int pad;

    public readonly int Length => length;
    public readonly int Size => size;
    public readonly int Count => encoder.PopCount();

    [SkipLocalsInit]
    public PaddedArray(int length, int sizeOf)
    {
        // Check if the sizeOf is not already aligned
        bool needsPad16 = (1 << (sizeOf & 16)) <= 5 && sizeOf < 16;
        bool needsPad32 = (1 << (sizeOf & 32)) <= 7 && sizeOf > 16 && sizeOf < 32;
        bool needsPad64 = (1 << (sizeOf & 64)) <= 9 && sizeOf > 32 && sizeOf < 64;

        // Calculate the required padding to align properly
        if (needsPad64) pad = 64 - (sizeOf & (64 - 1));
        else if (needsPad32) pad = 32 - (sizeOf & (32 - 1));
        else if (needsPad16) pad = 16 - (sizeOf & (16 - 1));
        else pad = 0; // Already aligned or no padding needed

        this.length = length;
        elementSize = sizeOf;
        indexOffset = elementSize + pad;
        size = indexOffset * length;

        buffer = (byte*)Marshal.AllocHGlobal(size);
        encoder = new IndexEncoder(length);

        Unsafe.InitBlock(buffer, 0, (uint)size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Write<T>(T data, int index) where T : unmanaged
    {
        if (index < 0 || index >= length) throw new IndexOutOfRangeException();
        if (encoder.IsOccupied(index)) Console.WriteLine($"Index {index} had something allocated here and was overwritten."); // add debug later

        byte* destination = buffer + (indexOffset * index);
        Unsafe.CopyBlock(destination, &data, (uint)elementSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe T* Read<T>(int index) where T : unmanaged
    {
        if (index < 0 || index >= length) throw new IndexOutOfRangeException();

        byte* source = buffer + (indexOffset * index);
        return (T*)source;
    }

    public ref T Get<T>(int index) where T : unmanaged
    {
        return ref *Read<T>(index);
    }

    public int Add<T>(T data) where T : unmanaged
    {
        if (encoder.PopCount() == length) return -1;

        int index = encoder.FirstFreeBit();

        Write(data, index);
        encoder.Set(index);
        return index;
    }

    public bool Remove<T>(T item, out int index) where T : unmanaged
    {
        if (Contains(item, out index) && encoder.IsOccupied(index))
        {
            encoder.Unset(index);
            return true;
        }

        index = -1;
        return false;
    }

    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int memcmp(void* ptr1, void* ptr2, UIntPtr count); // byte comparison since == doesnt work
    public bool Contains<T>(T item, out int index) where T : unmanaged
    {
        byte* itemPtr = (byte*)&item; // pointer to address
        int i = 0;
        index = -1;
        // 4 unrolled to match 16 byte simd
        for (; i <= length - 4; i += 4)
        {
            byte* source1 = buffer + (indexOffset * i);
            byte* source2 = buffer + (indexOffset * (i + 1));
            byte* source3 = buffer + (indexOffset * (i + 2));
            byte* source4 = buffer + (indexOffset * (i + 3));

            if (memcmp(itemPtr, source1, (UIntPtr)sizeof(T)) == 0) { index = i; return true; }
            if (memcmp(itemPtr, source2, (UIntPtr)sizeof(T)) == 0) { index = i + 1; return true; }
            if (memcmp(itemPtr, source3, (UIntPtr)sizeof(T)) == 0) { index = i + 2; return true; }
            if (memcmp(itemPtr, source4, (UIntPtr)sizeof(T)) == 0) { index = i + 3; return true; }
        }

        // ensure we catch the final length if not aligned perfectly
        if (i < Length)
        {
            for (; i < length; i++)
            {
                byte* source = buffer + (indexOffset * i);
                if (memcmp(itemPtr, source, (UIntPtr)sizeof(T)) == 0)
                {
                    index = i;
                    return true;
                }

            }
        }
        return false;
    }

    public void Clear()
    {
        encoder.Clear();
        Unsafe.InitBlock(buffer, 0, (uint)size);
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

//TODO debug reminder
// debug legend
/*
    ALL - flood warning

    UNSAFE
    DATASTRUCTURES
    ECS
    ENGINE
    WINDOW
    RENDER
    INPUT

    LOG
    WARN
    ERROR
    CRASH
    CORRUPTS
*/
