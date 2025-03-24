namespace SliLib.ECS;

public class Chunk(ComponentSetTemplate data, int index)
{
    private Stack<int> free = new(1);
    private ComponentSet Components = new(data.Clone());
    private int[] Ents = new int[data.Capacity]; // index = local id | stored int = global id
    public int Index { get; init; } = index;
    public int Count { get; private set; } = 0;
    public int ChunkSize { get => Components.SizeOfSet + (sizeof(int) * Components.ArrayCaps); }

    public int AddEntity(EntityInfo info)
    {
        int index;

        if (free.Count > 0)
        {
            index = free.Pop();

            info.LocalId = index;
            info.ChunkIndex = Index;

            return index;
        }

        if (Count == Components.ArrayCaps) return -1; // full

        index = Count++;
        Ents[index] = info.Id;

        info.LocalId = index;
        info.ChunkIndex = Index;

        return index;
    }

    public int RemoveEntity(EntityInfo info)
    {
        if (!Valid(info.LocalId)) return -1; // not in this chunk OOB

        if (Ents[info.LocalId] != info.Id) return -2; // bad matchup

        free.Push(info.LocalId);
        Ents[info.LocalId] = -1;
        info.LocalId = -1;
        info.ChunkIndex = -1;

        return 0; // success
    }

    public ComponentMemory<T> AccessArray<T>() where T : struct
    {
        return Components.AccessArray<T>();
    }

    private bool Valid(int index)
    {
        return index >= 0 && index < Count;
    }
}
