namespace SliLib.ECS;

public class Archetype
{
    private static int archCount = 0;
    private static int Increment() => ++archCount;
    public int Id { get; }

    private Dictionary<Type, IComponentArray> arrays;
    private Dictionary<int, int> globalToLocal;
    private Stack<int> openSlots;
    private int count;

    public int Count => count;

    public Archetype()
    {
        Id = Increment();
        arrays = [];
        openSlots = [];
        globalToLocal = [];
        count = 0;
    }

    public Archetype(Dictionary<Type, IComponentArray> array)
    {
        Id = Increment();
        arrays = array;
        openSlots = [];
        globalToLocal = [];
        count = 0;
    }

    public int AddEnt(int entIndex) // returns local index
    {
        int index;
        if (openSlots.Count != 0)
        {
            index = openSlots.Pop();
            // no generation needed since index is already populated
        }
        else
        {
            index = count;
            SetDefaults(index);
        }

        globalToLocal.Add(entIndex, index);
        count++;
        return index;
    }

    public bool RemoveEnt(int entIndex) // removes based on global index
    {
        if (globalToLocal.TryGetValue(entIndex, out var index))
        {
            openSlots.Push(index);
            globalToLocal.Remove(entIndex);
            ResetDefaults(index);

            count--;
            return true;
        }

        return false;
    }

    public BoxedEnt BoxEntity(int entIndex)
    {
        if (!ContainsEnt(entIndex))
        {
            throw new ArgumentException($"Entity {entIndex} cannot be boxed because it does not exist in this archetype.");
        }

        int local = GetLocalIndex(entIndex);

        BoxedEnt ent = new(entIndex, local);
        ent.BoxArchComps(this); // store local data

        RemoveEnt(entIndex); // clear local data
        return ent;
    }

    public void UnBoxEnt(BoxedEnt box)
    {
        int local = AddEnt(box.GlobalIndex); // set default components

        foreach (var (type, array) in arrays)
        {
            if (box.Contains(type))
            {
                object comp = box.GetBoxedComponent(type);
                array.SetBoxAtIndex(local, comp); // restores original entity data
            }
        }
    }

    public IEnumerable<Type> GetCompTypes() => arrays.Keys;
    public IEnumerable<int> GetLocalEnts() => globalToLocal.Values;

    public int GetLocalIndex(int entIndex)
    {
        if (globalToLocal.TryGetValue(entIndex, out var index))
        {
            return index;
        }

        else throw new KeyNotFoundException($"The entity {entIndex} is not stored here.");
    }

    public bool ContainsComp(Type type) => arrays.ContainsKey(type);
    public bool ContainsEnt(int entIndex) => globalToLocal.ContainsKey(entIndex);
    public bool IndexExists(int index)
    {
        if (count < index) return false;
        if (index < 0) return false;
        if (openSlots.Contains(index)) return false;
        return true;
    }

    public Archetype SetComponent<T>(int index, T component) where T : struct // gives values to components at index
    {
        if (IndexExists(index) && ContainsComp(typeof(T)))
            EnsureArray<T>()[index] = component;

        return this;
    }

    public ref T GetComponent<T>(int index) where T : struct
    {
        return ref EnsureArray<T>().Ref(index);
    }

    private ComponentArray<T> EnsureArray<T>() where T : struct
    {
        if (!arrays.TryGetValue(typeof(T), out var array) || array is not ComponentArray<T> typedArray)
            throw new InvalidOperationException($"No ComponentArray found for type {typeof(T).Name}.");
        return typedArray;
    }
    public IComponentArray GetArray(Type type)
    {
        if (!arrays.TryGetValue(type, out var array))
            throw new InvalidOperationException($"No ComponentArray found for type {type.Name}.");

        return array;
    }

    public IEnumerable<(Type Type, IComponentArray Array)> GetArrays()
    {
        foreach (var kvp in arrays)
        {
            yield return (kvp.Key, kvp.Value);
        }
    }

    private void SetDefaults(int index) // for first add only
    {
        foreach (var kvp in arrays)
        {
            int compIndex = kvp.Value.AddDefault();

            if (index != compIndex)
            {
                throw new InvalidDataException($"Index mismatch: {index} vs {compIndex} for {kvp.Key.Name}");
            }
        }
    }

    private void ResetDefaults(int index)
    {
        foreach (var kvp in arrays)
        {
            kvp.Value.SetDefault(index);
        }
    }

    public override string ToString()
    {
        var types = string.Join('|', arrays.Keys);
        return $"ID = {Id} Archetype of composition: {types} : was generated!";
    }
}
