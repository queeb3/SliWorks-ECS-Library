namespace SliLib.ECS;

internal class MaskContainer
{
    public int[] Contains;
    public int[] ContainedIn;
    private int containsCap;
    private int containedCap;
    private int containsCount;
    private int containedCount;

    public MaskContainer(int capacity = 1)
    {
        containsCap = capacity;
        containedCap = capacity;
        Contains = new int[containsCap];
        ContainedIn = new int[containedCap];
    }

    public void AddContains(int contains)
    {
        if (containsCount == containsCap)
        {
            containsCap++;
            Array.Resize(ref Contains, containsCap);
        }
        Contains[containsCount++] = contains;
    }

    public void AddContained(int isContainedIn)
    {
        if (containedCount == containedCap)
        {
            containedCap++;
            Array.Resize(ref ContainedIn, containedCap);
        }
        ContainedIn[containedCount++] = isContainedIn;
    }
}

internal record MaskInfo
{
    public readonly int Id;
    public readonly ChunkMask Mask;

    public MaskInfo(int id, ChunkMask mask)
    {
        Id = id;
        Mask = mask;
    }
}

public class MaskCacher
{
    private Dictionary<ChunkMask, int> lookup;
    private MaskInfo[] masks;
    private MaskContainer[] containment;

    public int Capacity { get; private set; }
    public int Count { get; private set; }
    private int expandBy;

    public MaskCacher(int capacity = 64)
    {
        Capacity = capacity;
        expandBy = Capacity;
        lookup = new(Capacity);
        masks = new MaskInfo[Capacity];
        containment = new MaskContainer[Capacity];
    }

    public void Add(ChunkMask mask)
    {
        if (Count >= Capacity) Expand();

        if (MaskExists(mask)) return;

        var id = Count++;
        var info = new MaskInfo(id, mask);
        masks[id] = info;
        containment[id] = new();
        lookup[mask] = id;

        Console.WriteLine($"New mask: {id} = {mask.PrintChunkBits(0)}");

        CheckForContainment(info);
    }

    public Span<ChunkMask> GetContains(ChunkMask mask)
    {
        if (!lookup.TryGetValue(mask, out var id))
        {
            return [];
        }

        var contains = containment[id];
        var ids = contains.Contains;
        var span = new ChunkMask[ids.Length].AsSpan();
        for (int i = 0; i < span.Length; i++)
        {
            span[i] = masks[ids[i]].Mask;
        }
        return span;
    }

    public Span<ChunkMask> GetContainedIn(ChunkMask mask)
    {
        if (!lookup.TryGetValue(mask, out var id))
        {
            Add(mask);
        }

        var contained = containment[id];
        var ids = contained.ContainedIn;
        var span = new ChunkMask[ids.Length].AsSpan();
        for (int i = 0; i < span.Length; i++)
        {
            span[i] = masks[ids[i]].Mask;
        }
        return span;
    }

    public bool MaskExists(ChunkMask mask) => lookup.ContainsKey(mask);

    public int GetMaskId(ChunkMask mask)
    {
        if (!MaskExists(mask))
            return -1;

        return lookup[mask];
    }

    private void CheckForContainment(MaskInfo newMask)
    {
        for (int i = 0; i < Count; i++)
        {
            var existingMask = masks[i]; // must check all masks to ensure all containments are covered
            if (existingMask.Id == newMask.Id) continue; // skip the new mask when it hits its id

            // does any existing mask contain the new mask
            if (existingMask.Mask.Contains(newMask.Mask))
            {
                containment[existingMask.Id].AddContains(newMask.Id);
                containment[newMask.Id].AddContained(existingMask.Id);
                Console.WriteLine($"mask {existingMask.Id} contains mask {newMask.Id}");
            }

            // does the new mask contain any existing mask
            if (newMask.Mask.Contains(existingMask.Mask))
            {
                containment[newMask.Id].AddContains(existingMask.Id);
                containment[existingMask.Id].AddContained(newMask.Id);
                Console.WriteLine($"mask {newMask.Id} contains mask {existingMask.Id}");
            }
        }
    }

    private void Expand()
    {
        Capacity += expandBy;
        Array.Resize(ref masks, Capacity);
        Array.Resize(ref containment, Capacity);
    }
}
