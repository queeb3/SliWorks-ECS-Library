using System.Runtime.CompilerServices;

namespace SliLib.ECS;

public class ComponentRegister
{
    private readonly Array[] componentArrays;
    private readonly ComponentInfo[] meta;
    private readonly Dictionary<Type, int> indexer;
    private readonly Chunk maskManager; // handles the bit positioning of components and chunkcode creation
    public int Capacity { get; private set; }
    public int Count { get; private set; }

    public ComponentRegister(int capacity = 256)
    {
        componentArrays = new Array[capacity];
        meta = new ComponentInfo[capacity];
        indexer = new(capacity);
        Capacity = capacity;
        maskManager = new();
        Count = 0;
    }

    public void Reg<T>(int componentCapacity = 256) where T : struct
    {
        if (!ValidateStruct<T>())
            throw new InvalidOperationException($"{typeof(T)} contains reference fields!");

        var size = Unsafe.SizeOf<T>();
        var type = typeof(T);
        var id = Count++;

        if (!ValidIndex(id))
            throw new IndexOutOfRangeException($"Component Register ran out of space to store more components, increase the component capacity. Current: {Capacity}.");

        indexer[type] = id;
        meta[id] = new ComponentInfo() with
        {
            Id = id,
            Size = size,
            Type = type,
            Code = maskManager.Register(id)
        };

        componentArrays[id] = new T[componentCapacity];
    }

    public ComponentDataTransfer GetComponentData(ChunkMask mask)
    {
        var codes = mask.Codes();
        int ctr = 0;
        Array[] arr = new Array[codes.Count()];
        ComponentInfo[] inf = new ComponentInfo[codes.Count()];


        foreach (var code in codes)
        {
            var id = Chunk.CodeToId(code); // component ids also index to their stored locations

            if (!maskManager.IdActive(id))
                throw new InvalidOperationException(); // should this be continue instead? if so how will i know it skipped a bad component?

            arr[ctr] = (Array)componentArrays[id].Clone();
            inf[ctr] = meta[id];
            ctr++;
        }

        return new ComponentDataTransfer(arr, inf);
    }

    // -----------------------------------Info Access Methods---------------------------------------------

    public ref ComponentInfo GetInfo(int id)
    {
        if (!RegisteredIndex(id))
            throw new InvalidOperationException($"Component Id {id} is not within bounds of currently registered components.");

        return ref meta[id];
    }

    public ref ComponentInfo GetInfo<T>() where T : struct
    {
        var type = typeof(T);
        if (!indexer.TryGetValue(type, out var index))
            throw new KeyNotFoundException($"Cannot find {type} because it does not exist in register.");

        return ref meta[index];
    }

    public ref ComponentInfo GetInfo(Type type)
    {
        if (!indexer.TryGetValue(type, out var index))
            throw new KeyNotFoundException($"Cannot find {type} because it does not exist in register.");

        return ref meta[index];
    }

    public ref ComponentInfo GetInfo(ChunkCode code)
    {
        var index = Chunk.CodeToId(code);
        if (!RegisteredIndex(index))
            throw new InvalidOperationException($"Component Code {code} does not exist in currently registered components.");

        return ref meta[index];
    }

    // ---------------------------------Utility Methods------------------------------------

    public static bool ValidateStruct<T>() where T : struct => ValidateType(typeof(T));
    private static bool ValidateType(Type type)
    {
        foreach (var field in type.GetFields(System.Reflection.BindingFlags.Instance |
                                             System.Reflection.BindingFlags.Public |
                                             System.Reflection.BindingFlags.NonPublic))
        {
            if (!field.FieldType.IsValueType) return false;
            if (field.FieldType.IsPrimitive) continue;
            if (!ValidateType(field.FieldType)) return false; // Recursively check nested structs
        }
        return true;
    }

    private bool RegisteredIndex(int index) => index >= 0 && index < Count;
    private bool ValidIndex(int index) => index >= 0 && index < Capacity;
}
