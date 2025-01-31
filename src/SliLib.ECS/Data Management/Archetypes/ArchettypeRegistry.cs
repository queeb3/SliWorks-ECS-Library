namespace SliLib.ECS;

using SliLib.Tools.Debug;

public class ArchetypeRegistry
{
    private Dictionary<ChunkMask, Archetype> archetypes;

    public ArchetypeRegistry()
    {
        archetypes = new(1);
    }

    public bool Add(ChunkMask mask, Archetype arch)
    {
        return archetypes.TryAdd(mask, arch);
    }

    public Archetype GetExact(ChunkMask mask)
    {
        if (archetypes.TryGetValue(mask, out var arch))
        {
            return arch;
        }

        throw new KeyNotFoundException($"The chunk mask does not exist."); // no point in showing type since its bit related
    }

    public IEnumerable<Archetype> GetMatching(ChunkMask mask)
    {
        foreach (var key in archetypes.Keys)
        {
            if (key.Contains(mask)) yield return archetypes[key];
        }
    }

    public Archetype GetOrCreate(ChunkMask mask, IEnumerable<Type> types) // createFunc is used to allow generics for components
    {
        if (!archetypes.TryGetValue(mask, out var arch))
        {
            arch = GenerateArchetype(types);
            archetypes[mask] = arch;
        }
        return arch;
    }

    public IEnumerable<Archetype> GetEnumerator() => archetypes.Values;

    public void MoveEnt(int ent, ChunkMask mask1, ChunkMask mask2)
    {
        Archetype a1 = GetExact(mask1);
        Archetype a2 = GetExact(mask2);

        if (a1.ContainsEnt(ent))
        {
            BoxedEnt box = a1.BoxEntity(ent);
            a2.UnBoxEnt(box);
        }
    }

    private static Archetype GenerateArchetype(IEnumerable<Type> types)
    {
        Dictionary<Type, IComponentArray> dict = [];

        foreach (var type in types.Distinct())
        {
            if (!type.IsValueType)
                throw new ArgumentException($"Type {type.Name} must be a struct to be used as a component.");

            var array = typeof(ComponentArray<>).MakeGenericType(type);
            var componentArray = Activator.CreateInstance(array) as IComponentArray
                ?? throw new InvalidOperationException($"Failed to create ComponentArray for type {type.Name}.");

            dict.Add(type, componentArray);
        }


        Archetype arch = new(dict);
        Log.General(arch.ToString());
        return arch;
    }
}
