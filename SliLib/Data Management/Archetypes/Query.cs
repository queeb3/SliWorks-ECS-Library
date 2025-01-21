namespace SliLib.Query;

using SliLib.Archetypes;

public class Query
{
    public IEnumerable<Archetype> Matches;
    private HashSet<Type> components;

    public Query(IEnumerable<Type> components)
    {
        var mask = Nexus.Entity.Components.GenerateMaskFromTypes(components);
        Matches = Nexus.Entity.Archetypes.GetMatching(mask);
        this.components = [.. components];
    }
    public Query()
    {
        Matches = [];
        components = [];
    }

    public void UpdateQuery()
    {
        var mask = Nexus.Entity.Components.GenerateMaskFromTypes(components);
        Matches = Nexus.Entity.Archetypes.GetMatching(mask);
    }

    public ref T GetComponent<T>(int ent, Archetype arch) where T : struct
    {
        return ref arch.GetComponent<T>(ent);
    }

    public Query With<T>() where T : struct
    {
        components.Add(typeof(T));
        UpdateQuery();
        return this;
    }

    public void Execute(Action<Archetype, int> action)
    {
        foreach (var arch in Matches)
        {
            for (int i = 0; i < arch.Count; i++)
            {
                action(arch, i);
            }
        }
    }
}
