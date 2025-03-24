using System.Runtime.InteropServices;

namespace SliLib.DataStructures;

internal unsafe struct DirectArray
{
    private byte* buffer;
    private int length;
    private int elementSize;
    private int size;

    public int Length => length;

    public DirectArray(int capacity, int sizeOf)
    {
        elementSize = sizeOf;
        length = capacity;

        size = length * elementSize;

        buffer = (byte*)Marshal.AllocHGlobal(size);
    }
}
