namespace SliLib.DataStructures;

internal unsafe struct ArrayMeta
{
    public void* Address;
    public AllocType AllocType;
    public int Size;
    public int References;
}
