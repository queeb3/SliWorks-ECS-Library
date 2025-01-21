namespace SliLib.Systems;

public class SystemsList
{
    private SortedList<int, BaseSystem> Systems;

    public SystemsList()
    {
        Systems = [];

        Console.WriteLine("SystemList Initialized...");
    }

    public bool Add(BaseSystem system)
    {
        if (Systems.ContainsKey(system.Priority))
        {
            Console.WriteLine($"A system with priority {system.Priority} already exists.");
        }

        return Systems.TryAdd(system.Priority, system);
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
        return Systems.Remove(system.Priority);
    }

    public IEnumerable<BaseSystem> GetSystems()
    {
        return Systems.Values;
    }
}
