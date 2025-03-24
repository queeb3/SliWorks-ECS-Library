namespace SliLib.ECS;

public class ArchetypeRegistry
{
    private readonly ComponentRegister CR;

    private readonly Dictionary<int, EntityInfo> influencedEntities; // ent register id to ents index
    private readonly Queue<int> entDumpster;

    private readonly MaskCacher maskCache;
    private ArchInfo[] archs;

    public int Count { get; private set; }
    public int Capacity { get; private set; }

    public int EntCount { get; private set; }
    private int callsTillClean;

    public ArchetypeRegistry(ComponentRegister register, int capacity = 64, int entsCap = 1024)
    {
        CR = register;

        Capacity = capacity;
        archs = new ArchInfo[Capacity];
        maskCache = new(Capacity);
        Count = 0;

        influencedEntities = new(entsCap);
        EntCount = 0;

        entDumpster = new(12000);
    }

    public EntityInfo AddEntity(EntityInfo entity, ChunkMask mask)
    {
        if (!CR.ValidMask(mask)) return entity;
        if (influencedEntities.ContainsKey(entity.Id)) return entity;

        // must always invalidate bad data just incase it is shared between other ARs
        var newInfo = new EntityInfo(entity.Id);
        var arch = GetArchetype(mask).Instance;

        influencedEntities[newInfo.Id] = newInfo;
        arch.Add(newInfo);
        EntCount++;

        EmptyDumpster();
        callsTillClean++;
        return newInfo;
    }

    public void RemoveEntity(int entity)
    {
        if (!influencedEntities.TryGetValue(entity, out var info))
        {
            return;
        }

        entDumpster.Enqueue(entity);
        EmptyDumpster();
    }

    public EntityInfo GetEntityInfo(int entity)
    {
        if (!influencedEntities.TryGetValue(entity, out var info))
        {
            return new(-1);
        }

        return info;
    }

    public Archetype GetEntityArchetype(EntityInfo entity)
    {
        return archs[entity.ArchetypeId].Instance;
    }

    public bool HasEntity(EntityInfo entity)
    {
        return influencedEntities.ContainsKey(entity.Id);
    }

    public ref ArchInfo GetArchetype(ChunkMask mask)
    {
        var maskIndex = maskCache.GetMaskId(mask);
        if (maskIndex == -1)
        {
            return ref archs[GenerateNewArchetype(mask)];
        }
        else if (archs[maskIndex] is null)
        {
            return ref archs[GenerateNewArchetype(mask)];
        }

        return ref archs[maskIndex];
    }

    public Query Query(ChunkMask mask)
    {
        var masks = maskCache.GetContainedIn(mask);
        var len = masks.Length;
        var query = new Query(this, mask, true);

        for (int i = 0; i < len; i++)
        {
            ref var info = ref GetArchetype(masks[i]);
            if (info is null || i >= Count) continue;
            ref var chunk = ref info.Instance.QueryActiveChunks();
            query.Add(ref chunk, ref info);
        }
        return query;
    }

    private int GenerateNewArchetype(ChunkMask mask) // generates archetype when one is not found
    {
        var info = new ArchInfo(Count++, CR.GenerateTemplate(mask));

        if (Count == Capacity) ExpandArchs();

        archs[info.Id] = info;
        maskCache.Add(mask);

        return info.Id;
    }

    private void EmptyDumpster()
    {
        var itr = entDumpster.Count;
        if (itr >= 10000 || callsTillClean >= 20000)
        {
            for (int i = 0; i < itr; i++)
            {
                var ent = entDumpster.Dequeue();
                var info = influencedEntities[ent];
                var state = archs[info.ArchetypeId].Instance.Remove(info);
                if (state == 0) influencedEntities.Remove(ent);
            }

            callsTillClean = 0;
        }

        // // state legend:
        // // 0 = success
        // // 1 = out of range
        // // 2 = entity is corrupted somehow
    }

    private void ExpandArchs()
    {
        var newACap = Capacity * 2;

        Array.Resize(ref archs, newACap);

        Capacity = newACap;
    }
}
