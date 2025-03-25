namespace SliLib.DataStructures;

internal unsafe struct AllocatorMetaInfo
{
    public ArrayType Type;
    public int Index;

    public int Size;

    public bool Freed;

    public AllocatorMetaInfo(ArrayType type, int index, int size, bool freed = false)
    {
        Type = type;
        Size = size;
        Index = index;
        Freed = freed;
    }
}
