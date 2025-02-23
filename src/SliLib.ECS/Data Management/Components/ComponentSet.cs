namespace SliLib.ECS;

public record ComponentDataTransfer
{
    public ComponentInfo[] Infos;
    public Array[] Arrays;

    // internally created to prevent bad data allocations because info and arrays must be aligned by index
    internal ComponentDataTransfer(Array[] arrays, ComponentInfo[] infos)
    {
        Infos = infos;
        Arrays = arrays;
    }
}

public class ComponentSet
{
    private readonly Array[] arrays; // the actual data stored, meant to be used in systems
    private readonly ComponentInfo[] componentInfos; // slow query for specific component info
    private readonly Dictionary<Type, int> indexer;

    public ChunkMask Mask { get; init; } // fast query
    public int Count { get; init; }
    public int SizeOfSet { get; init; } // all component sizes not including all class properies.

    public ComponentSet(ComponentDataTransfer transfer)
    {
        var length = transfer.Arrays.Length;
        int totalSize = 0;
        Count = 0;


        indexer = new(length);
        arrays = new Array[length];
        componentInfos = new ComponentInfo[length];

        foreach (var info in transfer.Infos)
        {
            var arr = transfer.Arrays[Count];

            arrays[Count] = arr;
            totalSize += info.Size * arr.Length;

            componentInfos[Count] = transfer.Infos[Count];
            indexer[info.Type] = Count++;
        }

        SizeOfSet = totalSize;
    }

    public ComponentMemory<T> AccessArray<T>() where T : struct
    {
        if (!indexer.TryGetValue(typeof(T), out var index))
        {
            throw new KeyNotFoundException();
        }

        return new(ref arrays[index]);
    }
}
