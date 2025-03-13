namespace SliLib.ECS;

public class ArchetypeRegistry
{
    private readonly ComponentRegister CR;

    private readonly Dictionary<int, EntityInfo> influencedEntities; // ent register id to ents index
    private readonly Dictionary<ChunkMask, int> maskIndexer;
    private ArchInfo[] archs;

    public int Count { get; private set; }
    public int Capacity { get; private set; }

    public int EntCount { get; private set; }

    public ArchetypeRegistry(ComponentRegister register, int capacity = 64, int entsCap = 1024)
    {
        CR = register;

        Capacity = capacity;
        archs = new ArchInfo[Capacity];
        maskIndexer = new(Capacity);
        Count = 0;

        influencedEntities = new(entsCap);
        EntCount = 0;
    }

    public EntityInfo AddEntity(EntityInfo info, ChunkMask mask)
    {
        if (!CR.ValidMask(mask)) return info;
        if (influencedEntities.ContainsKey(info.Id)) return info;

        influencedEntities[info.Id] = info;
        EntCount++;

        var arch = GetArchetype(mask);
        arch.Add(info);
        return info;
    }

    public EntityInfo GetEntityInfo(int entity)
    {
        return influencedEntities[entity];
    }

    public Archetype GetEntityArchetype(EntityInfo entity)
    {
        return archs[entity.ArchetypeId].Instance;
    }
    public Archetype GetEntityArchetype(int entity)
    {
        return archs[influencedEntities[entity].ArchetypeId].Instance;
    }

    public bool HasEntity(EntityInfo entity)
    {
        return influencedEntities.ContainsKey(entity.Id);
    }

    public Archetype GetArchetype(ChunkMask mask)
    {
        if (!maskIndexer.TryGetValue(mask, out var index))
        {
            return archs[GenerateNewArchetype(mask)].Instance;
        }

        return archs[index].Instance;
    }

    private int GenerateNewArchetype(ChunkMask mask) // generates archetype when one is not found
    {
        var info = new ArchInfo(Count++, CR.GenerateTemplate(mask));

        if (Count == Capacity) ExpandArchs();

        archs[info.Id] = info;
        maskIndexer[mask] = info.Id;

        return info.Id;
    }

    private void ExpandArchs()
    {
        var newACap = Capacity * 2;

        Array.Resize(ref archs, newACap);

        Capacity = newACap;
    }
}
