using System.Diagnostics;
using SliLib.ECS;
class Program
{
    public static void Main(string[] args)
    {
        var cr = new ComponentRegister();
        cr.Reg<Position>().Reg<Velocity>();

        var er = new EntityRegister();
        var ar = new ArchetypeRegistry(cr);

        var ee = new EntityEditor(ar);
        var eb = new EntityBuilder(er, cr);

        var mask = new ChunkMaskBuilder(cr).Add<Position>().Add<Velocity>().Return();
        var query = ar.Query(mask);

        var prefab = new Prefab<Position, Velocity>(new(5, 6, 8), new(1, 1, 1));
        for (int i = 0; i < 500; i++) prefab.UseFab(eb, ar);

        var shard = new Shard(query);

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++) shard.Process(c => Move(c));
        Console.WriteLine(sw.Elapsed);
        sw.Stop();
    }

    public static void Move(Chunk c)
    {
        var p = c.AccessArray<Position>();
        var v = c.AccessArray<Velocity>();

        for (int i = 0; i < p.Length; i++)
        {
            var p1 = p[i].x += v[i].x;
            var p2 = p[i].y += v[i].y;
            var p3 = p[i].z += v[i].z;
            if (i == 0) Console.WriteLine($"Entity {i}: ({p1}, {p2}, {p3})");
        }

    }
}


public struct Position(float x, float y, float z) { public float x = x, y = y, z = z; }
public struct Velocity(float x, float y, float z) { public float x = x, y = y, z = z; }
