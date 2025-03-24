namespace SliLib.DataStructures;

internal unsafe struct ArrayArena
{
    private IndexEncoder stackEnc;
    private IndexEncoder heapEnc;
    private ArrayMeta* stackAllocs;
    private ArrayMeta* heapAllocs;
}
