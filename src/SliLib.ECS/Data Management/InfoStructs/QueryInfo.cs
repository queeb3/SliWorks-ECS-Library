namespace SliLib.ECS;

public readonly struct QueryInfo
{
    private readonly static Dictionary<ChunkMask, QueryInfo> queries = new(32); // pool of cached queries
    public readonly int[] Included;
    public readonly int[] Excluded;
    public readonly ChunkMask Mask;
    // public readonly Query? Query; // original query

    private QueryInfo(int[] included, int[] excluded)
    {
        Included = included;
        Excluded = excluded;

        Mask = GenerateMask(included, excluded);

        queries.TryAdd(Mask, this);
    }

    public static QueryInfo Request(int[] included, int[] excluded)
    {
        var mask = GenerateMask(included, excluded);

        if (queries.TryGetValue(mask, out var info))
        {
            return info;
        }
        else
        {
            // TODO create query method to generate a inc exc filter
            // Query = Query.Generate(included, excluded) or Query.Generate(mask)

            return new QueryInfo(included, excluded);
        }
    }

    private static ChunkMask GenerateMask(int[] included, int[] excluded)
    {
        return new(); // TODO add a static mask generator
    }
}
