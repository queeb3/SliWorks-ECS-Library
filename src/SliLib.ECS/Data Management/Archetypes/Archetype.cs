namespace SliLib.ECS;

// TODO add summary for danger of direct use on all hotpath methods
// hotpaths have minimal or no branches to ensure maximum acess speeds. will try and create helper
// classes to ease dev use and provide some safety nets like bad return or bypass code to jump to next
// item in a collection to prevent bad access


/// <summary>
/// Stores and handles all <see cref= "Chunk"/>s for a specific group of components and entities.
/// </summary>
public class Archetype
{
    /// <summary>
    /// Meta Id linked back to the ArchInfo index.
    /// </summary>
    public int Id { get; internal set; }

    private readonly ComponentSetTemplate Template;
    private BitIndexer indexer;

    private Chunk[] Chunks;
    public int Count { get; private set; }
    public int EntCount { get; private set; }
    public int Capacity { get; private set; }
    private int expandAmount;

    public int EntityCap { get => Template.Capacity * Count; }
    public int SizeOfArchetype { get => Chunks[0].ChunkSize * Count; }

    public Archetype(int id, ComponentSetTemplate template, int capacity = 256)
    {
        Id = id;
        Template = template;
        Capacity = capacity;
        expandAmount = capacity;

        Chunks = new Chunk[Capacity];
        indexer = new(Capacity);

        for (int i = 0; i < Capacity; i++)
        {
            Chunks[i] = new(template.Clone(), i);
            Count++;
        }

    }

    public void Add(EntityInfo info) // semi hotpath
    {
        var chunk = indexer.FindFreeIndex();
        if (chunk == Capacity - 1) Expand();

        var alreadyFull = Chunks[chunk].AddEntity(info);

        if (alreadyFull == -1)
        {
            indexer.Set(chunk);
            Add(info); // ensures that the entity will be added to the next chunk
        }

        EntCount++;
        info.ArchetypeId = Id;
    }

    public int Remove(EntityInfo info) // WIP - semi hotpath
    {
        var chunkIndex = info.ChunkIndex;

        indexer.Unset(chunkIndex);

        return Chunks[chunkIndex].RemoveEntity(info);
    }

    public ref T Get<T>(EntityInfo entity) where T : struct // semi hotpath
    {
        return ref Chunks[entity.ChunkIndex].AccessArray<T>()[entity.LocalId];
    }

    // semi hotpath - not meant to be used unless wanting to directly edit certain component indexes
    // without being in an iterator with just chunks
    public ComponentMemory<T> GetArray<T>(EntityInfo entity) where T : struct
    {
        if (!ValidChunk(entity.ChunkIndex))
            return new();

        return Chunks[entity.ChunkIndex].AccessArray<T>();
    }

    public ref Chunk this[int index]
    {
        get
        {
            if (ValidChunk(index)) return ref Chunks[index];
            else throw new IndexOutOfRangeException();
        }
    }

    public bool TryGetChunk(int chunkIndex, out Chunk chunk) // prefered for direct chunk access
    {
        if (!ValidChunk(chunkIndex))
        {
            chunk = new(Template, -1);
            return false;
        }

        chunk = Chunks[chunkIndex];
        return true;
    }

    public ref Chunk[] QueryActiveChunks()
    {
        return ref Chunks;
    }

    private bool ValidChunk(int index)
    {
        return index >= 0 && index < Capacity;
    }

    public void Expand() // very hot when adding new entities in bulk but seems to hold up quite well linearly
    {
        // slow expansion to reduce unnecessary iterations, this will generally only affect startup times
        var newCap = Capacity + expandAmount;

        Array.Resize(ref Chunks, newCap);

        // preallocate all new chunks ahead of use - causes more overhead blips but overall is a performance increase
        // when adding
        for (int i = Count; i < newCap; i++)
        {
            Chunks[i] = new(Template.Clone(), i);
            Count++;
        }

        Capacity = newCap;
        indexer.Expand(Capacity);
    }
}
