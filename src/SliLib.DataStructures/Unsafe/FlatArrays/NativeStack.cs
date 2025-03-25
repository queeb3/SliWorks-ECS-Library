namespace SliLib.DataStructures;

public unsafe struct NativeStack<T> where T : unmanaged
{
    private T* buffer;

    private int length;
    private int size;
    private int elementSize;
    private int count;

    public readonly int Count => count;

    public NativeStack(int capacity)
    {
        length = capacity;
        elementSize = sizeof(T);
        size = length * elementSize;

        buffer = (T*)Marshal.AllocHGlobal(size);
        Unsafe.InitBlock(buffer, 0, (uint)size);
    }

    public T Pop()
    {
        if (count > 0)
        {
            return buffer[--count];
        }

        return default;
    }

    public bool Push(T value)
    {
        if (count == length)
        {
            return false;
        }

        buffer[count++] = value;
        return true;
    }

    public void Free()
    {
        Marshal.FreeHGlobal((nint)buffer);
        buffer = null;
    }
}
