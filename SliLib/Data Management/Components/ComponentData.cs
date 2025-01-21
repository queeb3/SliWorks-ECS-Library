namespace SliLib.Components;

using SliLib.Chunks;

public class ComponentData // meta data for components
{
    public class Component(ChunkCode code, Type type, uint id)
    {
        public readonly ChunkCode Code = code;
        public readonly Type Type = type;
        public readonly uint Id = id;
    }

    private readonly Dictionary<uint, Component> iComponents;
    private readonly Dictionary<Type, Component> tComponents;
    private Chunk Chunk { get; }

    public ComponentData()
    {
        Chunk = new();
        iComponents = [];
        tComponents = [];
    }

    public ComponentData Add<T>(uint id) where T : struct
    {
        if (tComponents.ContainsKey(typeof(T)) || iComponents.ContainsKey(id))
        {
            throw new ArgumentException($"The component type {typeof(T).Name} or id {id} is already registered.");
        }

        var code = Chunk.Register<T>(id);
        var component = new Component(code, typeof(T), id);

        iComponents.Add(id, component);
        tComponents.Add(typeof(T), component);

        return this;
    }

    public Chunk GetChunk() => Chunk;
    public Component GetComponentInfo(uint id)
    {
        if (iComponents.TryGetValue(id, out var component))
            return component;

        throw new KeyNotFoundException($"No component found for ID {id}.");
    }
    public Component GetComponentInfo(Type type)
    {
        if (tComponents.TryGetValue(type, out var component))
            return component;

        throw new KeyNotFoundException($"No component found for Type {type.Name}.");
    }
    public Component GetComponentInfo<T>() where T : struct => GetComponentInfo(typeof(T));

    public bool ValidComponent(uint id)
    {
        if (!iComponents.TryGetValue(id, out var component))
            return false;

        if (!tComponents.TryGetValue(component.Type, out var typeComponent))
            return false;

        return typeComponent.Id == id && typeComponent.Type == component.Type;
    }

    public bool ValidComponent(Type type)
    {
        if (!tComponents.TryGetValue(type, out var component))
            return false;

        if (!iComponents.TryGetValue(component.Id, out var idComponent))
            return false;

        return idComponent.Type == type && idComponent.Id == component.Id;
    }

    public bool ValidComponent<T>() where T : struct => ValidComponent(typeof(T));

    public Type GetComponentType(uint id) => GetComponentInfo(id).Type;
    public uint GetComponentId<T>() where T : struct => GetComponentId(typeof(T));
    public uint GetComponentId(Type type)
    {
        if (!tComponents.TryGetValue(type, out var component))
        {
            throw new ArgumentException($"There is no component of type {type.Name}.");
        }

        return component.Id;
    }
    public ChunkCode GetComponentCode(uint id) => GetComponentInfo(id).Code;
    public ChunkCode GetComponentCode(Type type) => GetComponentInfo(type).Code;
    public ChunkCode GetComponentCode<T>() where T : struct => GetComponentCode(typeof(T));

    public int GetComponentBit(uint id) => GetComponentInfo(id).Code.Bit;
    public int GetComponentBit(Type type) => GetComponentInfo(type).Code.Bit;
    public int GetComponentBit<T>() where T : struct => GetComponentBit(typeof(T));

    public int GetComponentChunk(uint id) => GetComponentInfo(id).Code.Chunk;
    public int GetComponentChunk(Type type) => GetComponentInfo(type).Code.Chunk;
    public int GetComponentChunk<T>() where T : struct => GetComponentChunk(typeof(T));

    // ----------------------------ChunkMask Generation from meta data--------------------------------


    public ChunkMask GenerateMaskFromIds(IEnumerable<uint> ids)
    {
        var mask = new ChunkMask();

        foreach (var id in ids)
        {
            if (ValidComponent(id))
            {
                var code = GetComponentCode(id);
                mask.AddChunkCode(code);
            }
        }

        return mask;
    }

    public ChunkMask GenerateMaskFromTypes(IEnumerable<Type> types)
    {
        var mask = new ChunkMask();

        foreach (var type in types)
        {
            if (ValidComponent(type))
            {
                var code = GetComponentCode(type);
                mask.AddChunkCode(code);
            }
        }

        return mask;
    }

    public IEnumerable<Type> GetTypesFromMask(ChunkMask mask)
    {
        return mask.Codes()
            .Where(code => iComponents.Values.Any(c => c.Code == code))
            .Select(code => iComponents.Values.First(c => c.Code == code).Type);
    }
}
