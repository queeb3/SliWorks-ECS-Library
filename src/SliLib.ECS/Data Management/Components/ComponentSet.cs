using System.Runtime.CompilerServices;

namespace SliLib.ECS;


//NOTE: this is like direct control over the arrays via spans and ref and is dangerous if used
//without that in mind. i have placed some failsafes but please try and edit via archeypes
//with entity indexes instead unless you know what you are doing.
public class ComponentSet
{
    private readonly Array[] arrays;
    private readonly ComponentInfo[] componentInfos;
    private readonly Dictionary<Type, int> indexer;

    public ChunkMask Mask { get; init; }
    public int Count { get; init; }
    public int SizeOfSet { get; init; } // all component sizes not including all class fields.
    public int ArrayCaps { get; init; }

    public ComponentSet(ComponentSetTemplate transfer)
    {
        var length = transfer.Arrays.Length;
        int totalSize = 0;
        Count = 0;


        indexer = new(length);
        arrays = new Array[length];
        componentInfos = new ComponentInfo[length];
        Mask = transfer.Mask;

        foreach (var info in transfer.Infos)
        {
            var arr = transfer.Arrays[Count];

            arrays[Count] = arr;
            totalSize += info.Size * arr.Length;

            componentInfos[Count] = transfer.Infos[Count];
            indexer[info.Type] = Count++;
        }

        SizeOfSet = totalSize;
        ArrayCaps = transfer.Capacity;
    }

    public void Add<T>(int index, T data) where T : struct
    {
        if (!indexer.TryGetValue(typeof(T), out var i) || !Valid(index))
        {
            return;
        }

        Unsafe.As<T[]>(arrays[i])[index] = data;
    }

    public ref T Edit<T>(int index) where T : struct
    {
        if (!Valid(index))
        {
            Console.WriteLine($"The index: {index} was out of range when acessing Component {typeof(T).Name} by reference");
            Environment.Exit(1);
        }
        return ref AccessArray<T>()[index];
    }

    public ComponentMemory<T> AccessArray<T>() where T : struct
    {
        if (!indexer.TryGetValue(typeof(T), out var index))
        {
            return new ComponentMemory<T>();
        }

        return new(ref arrays[index]);
    }

    public override string ToString()
    {
        var list = "Contains Components: ";
        foreach (var comp in indexer.Keys)
        {
            list += comp.Name + " | ";
        }
        if (indexer.Count == 0) list = "This is an empty Set.";
        return list;
    }

    private bool Valid(int index)
    {
        return index <= ArrayCaps && index >= 0;
    }
}
