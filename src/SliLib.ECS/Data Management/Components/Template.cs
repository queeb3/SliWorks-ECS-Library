namespace SliLib.ECS;

public struct ComponentSetTemplate
{
    public ChunkMask Mask;
    public int Capacity;
    public ComponentInfo[] Infos;
    public Array[] Arrays;

    // internally created to prevent bad data allocations because info and arrays must be aligned by index
    internal ComponentSetTemplate(Array[] arrays, ComponentInfo[] infos, ChunkMask mask, int capacity)
    {
        Infos = infos;
        Arrays = arrays;
        Mask = mask;
        Capacity = capacity;
    }

    public ComponentSetTemplate Clone()
    {
        var newInfos = new ComponentInfo[Infos.Length];
        var newArrays = new Array[Arrays.Length];

        for (int i = 0; i < Infos.Length; i++)
        {
            newInfos[i] = Infos[i];
            newArrays[i] = (Array)Arrays[i].Clone();
        }

        return new ComponentSetTemplate(newArrays, newInfos, Mask, Capacity);
    }
}
