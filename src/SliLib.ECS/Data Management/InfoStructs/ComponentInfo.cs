namespace SliLib.ECS;

public record ComponentInfo
{
    public int Id { get; init; }
    public int Size { get; init; } // size in bytes
    public Type Type { get; init; }
    public ChunkCode Code { get; init; } // where in the chunkmask this component will always appear
}
