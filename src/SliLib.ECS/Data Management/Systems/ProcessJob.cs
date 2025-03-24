namespace SliLib.ECS;

public class Shard
{
    private Query query;

    public Shard(Query q)
    {
        query = q;
    }

    public void Process(Action<Chunk> logic)
    {
        var chunks = query.GetChunks();
        foreach (var c in chunks)
        {
            foreach (var chunk in c)
            {
                for (int i = 0; i < chunk.Count; i++)
                    logic(chunk);
            }
        }
    }
}
