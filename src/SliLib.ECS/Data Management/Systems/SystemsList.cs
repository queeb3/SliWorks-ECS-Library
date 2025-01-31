namespace SliLib.ECS;

using SliLib.Tools.Debug;

public class SystemsList
{
    private readonly SortedList<int, BaseSystem> SortedSystems;

    public SystemsList()
    {
        Log.General("Loading SystemList...");

        SortedSystems = [];

        Console.WriteLine("SystemList Loaded!");
    }

    public bool Add(BaseSystem system)
    {
        if (SortedSystems.ContainsKey(system.Priority))
        {
            Console.WriteLine($"A system with priority {system.Priority} already exists.");
        }

        Log.General($"System {system.GetType().Name} was registered!");

        return SortedSystems.TryAdd(system.Priority, system);
    }

    public void Add(params BaseSystem[] systems)
    {
        foreach (BaseSystem system in systems)
        {
            Add(system);
        }
    }

    public bool Remove(BaseSystem system)
    {
        return SortedSystems.Remove(system.Priority);
    }

    public IEnumerable<BaseSystem> GetSystems()
    {
        return SortedSystems.Values;
    }
}
