namespace SliLib.ECS;

public class Archetype
{
    public int Id { get; internal set; }

    private readonly ComponentSetTemplate Template;

    private Chunk[] Chunks;
    private readonly BitIndexer indexer;
    public int Count { get; private set; }
    public int EntCount { get; private set; }
    public int Capacity { get; private set; }


    public int EntityCap => Template.Capacity * Count;
    public int SizeOfArchetype() => (Chunks[0].Set.SizeOfSet * Count) + (EntityCap * sizeof(int));

    public Archetype(ComponentSetTemplate template, int capacity = 512)
    {
        Template = template;
        Capacity = capacity;
        Chunks = new Chunk[Capacity];
        for (int i = 0; i < capacity; i++)
        {
            Chunks[i] = new(template.Clone(), i);
            Count++;
        }

        indexer = new(capacity);
    }

    public void Add(EntityInfo info)
    {
        var chunk = indexer.FindFreeIndex();
        if (chunk == Capacity - 1) Expand();

        var full = Chunks[chunk].AddEntity(info);

        if (full == -1)
        {
            indexer.Set(chunk);
            Add(info);
        }
        EntCount++;
        info.ArchetypeId = Id;
    }

    public int Remove(EntityInfo info)
    {
        var chunkIndex = info.ChunkIndex;


        return Chunks[chunkIndex].RemoveEntity(info);
    }

    internal Chunk[] GetChunks() => Chunks;

    public ref T Get<T>(EntityInfo entity) where T : struct
    {
        return ref Chunks[entity.ChunkIndex].Set.Edit<T>(entity.LocalId);
    }

    public ComponentMemory<T> GetArray<T>(int chunkIndex) where T : struct
    {
        if (!ValidChunk(chunkIndex))
            return new();

        return Chunks[chunkIndex].Set.AccessArray<T>();
    }

    public ref Chunk this[int index]
    {
        get
        {
            if (ValidChunk(index)) return ref Chunks[index];
            else throw new IndexOutOfRangeException();
        }
    }

    public bool TryGetChunk(int chunkIndex, out Chunk chunk)
    {
        if (!ValidChunk(chunkIndex))
        {
            chunk = new(Template, -1);
            return false;
        }

        chunk = Chunks[chunkIndex];
        return true;
    }

    private bool ValidChunk(int index)
    {
        return index >= 0 && index < Capacity;
    }

    public void Expand()
    {
        var newCap = Capacity + 512; // slow expansion to reduce unneccessary iterations, this will generally only affect startup times

        Array.Resize(ref Chunks, newCap);

        for (int i = Count; i < newCap; i++)
        {
            Chunks[i] = new(Template.Clone(), i);
            Count++;
        }

        Capacity = newCap;
        indexer.Expand(Capacity);
    }
}
