namespace SliLib.Managers;

using SliLib.Entities;
using SliLib.Components;
using SliLib.Archetypes;

public class EntityManager
{
    public EntityArray Entities { get; init; }
    public ComponentData Components { get; init; }
    public ArchetypeRegistry Archetypes { get; init; }

    public EntityManager()
    {
        Console.WriteLine("Loading Entity Manager...");

        Entities = new();
        Components = new();
        Archetypes = new();

        Console.WriteLine("Entity Manager Initialized...");
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

        tempEnt ??= new(ent, 0); // if someone wants an empty ent for whatever reason ill allow it
        tempEnt.GlobalIndex = ent;

        var types = tempEnt.GetBoxedTypes();
        var mask = Components.GenerateMaskFromTypes(types);
        Entities.ChangeMask(ent, mask);

        // places new ent in its appropriate arch
        var arch = Archetypes.GetOrCreate(mask, types);
        arch.UnBoxEnt(tempEnt);

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
