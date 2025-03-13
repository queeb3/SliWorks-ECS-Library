namespace SliLib.ECS;

public class Chunk(ComponentSetTemplate data, int initialIndex)
{
    private Stack<int> free = new(1);
    public ComponentSet Set = new(data.Clone());
    public int[] Ents = new int[data.Capacity]; // index = local id | stored int = global id
    public int Index = initialIndex;
    public int Count { get; private set; } = 0;

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

        if (Count == Set.ArrayCaps) return -1; // full

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

        return 0;
    }

    public void AddComp<T>(int index, T data) where T : struct
    {
        if (!Valid(index)) return;
        Set.Edit<T>(index) = data;
    }

    private bool Valid(int index)
    {
        return index >= 0 && index < Count;
    }
}
