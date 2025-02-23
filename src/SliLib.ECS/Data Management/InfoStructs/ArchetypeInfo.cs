namespace SliLib.ECS;

public struct ArchInfo
{
    public readonly int Id;
    public readonly ChunkMask Mask; // component composition
    public int Index; // index of archetype for specific mask
}
