using System.Diagnostics;
using SliLib.ECS;
class Program
{
    public static void Main(string[] args)
    {
        var CR = new ComponentRegister(capacity: 16, compArrayCapacities: 64);
        CR.Reg<Position>().Reg<Velocity>().Reg<Rotation>();

        var ER = new EntityRegister(resizable: true);
        var AR = new ArchetypeRegistry(CR);
        var EE = new EntityEditor(AR);
        var EB = new EntityBuilder(ER, CR);

        var p1 = new Prefab<Position>(new Position() with { x = 1, y = 2, z = 3 });

        var sw = Stopwatch.StartNew();
        var lastTime = sw.Elapsed;
        var ctr = 0;

        for (int i = 0; i < 512 * 25000; i++)
        {
            ctr++;
            var e = p1.UseFab(EB, AR);
            if (ctr >= 500000)
            {
                // var ent = AR.GetEntityInfo(i);
                // Console.WriteLine($"Entity:{ent.Id} | Arch:{ent.ArchetypeId} | Chunk:{ent.ChunkIndex} | Local:{ent.LocalId}");
                Console.WriteLine($"Update: Ents:{ER.Count} | Blocks: {ER.BCount} | Chunks:{AR.GetEntityArchetype(e).Count} | Size:{AR.GetEntityArchetype(e).SizeOfArchetype()} | Time:{sw.Elapsed} | Difference:{sw.Elapsed - lastTime}");
                lastTime = sw.Elapsed;
                ctr = 0;
            }
        }
        sw.Stop();
        Console.WriteLine($"Update: Ents:{ER.Count} | Blocks: {ER.BCount} | Time:{sw.Elapsed} | Difference:{sw.Elapsed - lastTime}");
    }

}

public struct Position { public float x, y, z; }
public struct Velocity { public float x, y, z; }
public struct Rotation { public float x, y, z; }
