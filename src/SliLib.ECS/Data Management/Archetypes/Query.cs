namespace SliLib.ECS;

public class Query
{
    private readonly ArchetypeRegistry AR;
    public readonly ChunkMask Mask;
    private ArchInfo[] archs;
    private Chunk[][] queried;

    public int Capacity { get; private set; }
    public int Count { get; private set; }

    public Query(ArchetypeRegistry ar, ChunkMask mask)
    {
        AR = ar;
        Mask = mask;

        Capacity = 1;
        queried = new Chunk[Capacity][];
        archs = new ArchInfo[Capacity];
        Refresh();
    }

    internal Query(ArchetypeRegistry ar, ChunkMask mask, bool registry)
    {
        AR = ar;
        Mask = mask;

        Capacity = 1;
        queried = new Chunk[Capacity][];
        archs = new ArchInfo[Capacity];
    }

    internal void Add(ref Chunk[] chunks, ref ArchInfo arch)
    {
        if (Count == Capacity)
        {
            Capacity++;
            Array.Resize(ref archs, Capacity);
            Array.Resize(ref queried, Capacity);
        }
        archs[Count] = arch;
        queried[Count++] = chunks;
    }

    public Query Refresh()
    {
        return AR.Query(Mask);
    }

    public Chunk[][] GetChunks()
    {
        return queried;
    }
}
