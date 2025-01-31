namespace SliLib.ECS;

public class EntityManager
{
    public EntityArray Entities { get; init; }
    public ComponentData Components { get; init; }
    public ArchetypeRegistry Archetypes { get; init; }

    public EntityManager(EntityArray? entities = null, ComponentData? compData = null, ArchetypeRegistry? archetypes = null)
    {
        Console.WriteLine("Loading Entity Manager...");

        Entities = entities ?? new EntityArray();
        Components = compData ?? new ComponentData();
        Archetypes = archetypes ?? new ArchetypeRegistry();

        Console.WriteLine("Entity Manager Loaded!");
    }

    public EntityManager Reg<T>(uint id) where T : struct
    {
        Components.Add<T>(id);
        return this;
    }

    private BoxedEnt? tempEnt;

    public int CreateEnt()
    {
        int ent = Entities.Add();

        // allocate box or reuse
        tempEnt ??= new(ent, 0);
        tempEnt.GlobalIndex = ent;

        // places new ent in its appropriate arch
        var types = tempEnt.GetBoxedTypes();
        var mask = Components.GenerateMaskFromTypes(types);
        var arch = Archetypes.GetOrCreate(mask, types);
        arch.UnBoxEnt(tempEnt);

        // release box instance
        tempEnt = null;

        return ent;
    }

    public EntityManager StageComp<T>(T component) // chain add components to entity
    {
        tempEnt ??= new(0, 0);
        tempEnt.BoxComponent(typeof(T), component!);
        return this;
    }
}
