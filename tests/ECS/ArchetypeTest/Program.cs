using System.Diagnostics;
using SliLib.ECS;
class Program
{
    public static void Main(string[] args)
    {
        var CR = new ComponentRegister(capacity: 512, compArrayCapacities: 512);
        CR.Reg<Position>().Reg<Velocity>().Reg<Rotation>().Reg<Test1>().Reg<Test2>().Reg<Test3>().Reg<Test4>().Reg<Test5>()
        .Reg<Test6>().Reg<Test7>().Reg<Test8>().Reg<Test9>().Reg<Test10>().Reg<Test11>().Reg<Test12>();

        var ER = new EntityRegister();
        var AR = new ArchetypeRegistry(CR);

        var EE = new EntityEditor(AR);
        var EB = new EntityBuilder(ER, CR);

        var p1 = new Prefab<Position>(new Position() with { x = 1, y = 2, z = 3 });
        var p2 = new Prefab<Position, Velocity>(new Position() with { x = 0, y = 0, z = 0 }, new Velocity() with { x = 1, y = 4, z = 3 });
        var p3 = new Prefab<Position, Velocity, Rotation>(new(), new(), new());
        var p4 = new Prefab<Position, Velocity, Test1>(new(), new(), new());
        var p45 = new Prefab<Position, Velocity, Test1, Rotation>(new(), new(), new(), new());
        var p5 = new Prefab<Position, Velocity, Test2>(new(), new(), new());
        var p55 = new Prefab<Position, Velocity, Test2, Rotation>(new(), new(), new(), new());
        var p6 = new Prefab<Position, Velocity, Test3>(new(), new(), new());
        var p7 = new Prefab<Position, Velocity, Test4>(new(), new(), new());
        var p8 = new Prefab<Position, Velocity, Test5>(new(), new(), new());
        var p85 = new Prefab<Position, Velocity, Test5, Rotation>(new(), new(), new(), new());
        var p9 = new Prefab<Position, Velocity, Test6>(new(), new(), new());
        var p10 = new Prefab<Position, Velocity, Test7>(new(), new(), new());

        var mask = new ChunkMaskBuilder(CR).Add<Position>().Add<Velocity>().Return();
        var mask1 = new ChunkMaskBuilder(CR).Add<Test2>().Add<Rotation>().Return();
        var mask2 = new ChunkMaskBuilder(CR).Add<Position>().Add<Test1>().Return();
        var mask3 = new ChunkMaskBuilder(CR).Add<Position>().Add<Test5>().Return();
        var mask4 = new ChunkMaskBuilder(CR).Add<Rotation>().Return();

        var sw = Stopwatch.StartNew();
        var lastTime = sw.Elapsed;
        var ctr = 0;
        var ctrE = 0;

        p2.UseFab(EB, AR);

        EntityInfo e = new(-1);
        for (int i = 0; i < 2_500_001; i++)
        {
            ctr++;
            ctrE++;
            switch (new Random().Next(0, 12))
            {
                case 0:
                    e = p1.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 1:
                    e = p2.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 2:
                    e = p3.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 3:
                    e = p4.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 4:
                    e = p45.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 5:
                    e = p5.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 6:
                    e = p55.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 7:
                    e = p6.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 8:
                    e = p7.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 9:
                    e = p8.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 10:
                    e = p85.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 11:
                    e = p9.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
                case 12:
                    e = p10.UseFab(EB, AR);
                    if (ctrE == 5000000 / 10)
                    {
                        PrintArch(e, AR);
                    }
                    break;
            }

            if (ctrE == 5000000 / 10)
            {
                ctrE = 0;
            }

            if (ctr >= 500000)
            {
                // var ent = AR.GetEntityInfo(i);
                // Console.WriteLine($"Entity:{ent.Id} | Arch:{ent.ArchetypeId} | Chunk:{ent.ChunkIndex} | Local:{ent.LocalId}");
                Console.WriteLine($"Update: Ents:{ER.Count} | Blocks: {ER.BCount} | Time:{sw.Elapsed} | Difference:{sw.Elapsed - lastTime}");
                lastTime = sw.Elapsed;
                ctr = 0;
            }
        }
        sw.Stop();

        sw.Restart();
        var psuedoSys = AR.Query(mask);
        var psuedoSys1 = AR.Query(mask1);
        var psuedoSys2 = AR.Query(mask2);
        var psuedoSys3 = AR.Query(mask3);
        var psuedoSys4 = AR.Query(mask4);
        for (int i = 0; i < 500; i++)
        {
            psuedoSys = psuedoSys.Refresh();
            psuedoSys1 = psuedoSys1.Refresh();
            psuedoSys2 = psuedoSys2.Refresh();
            psuedoSys3 = psuedoSys3.Refresh();
            psuedoSys4 = psuedoSys4.Refresh();
        }

        sw.Stop();

        Console.WriteLine($"{sw.Elapsed} | {psuedoSys.Count},{psuedoSys.Capacity} | {psuedoSys1.Count},{psuedoSys1.Capacity} | {psuedoSys2.Count},{psuedoSys2.Capacity} | {psuedoSys3.Count},{psuedoSys3.Capacity} | {psuedoSys4.Count},{psuedoSys4.Capacity}");
    }

    static void PrintArch(EntityInfo ent, ArchetypeRegistry AR)
    {
        Console.WriteLine($"Update: {ent.ArchetypeId} | Chunks:{AR.GetEntityArchetype(ent).Count} | Size:{AR.GetEntityArchetype(ent).SizeOfArchetype}");
    }
}

public struct Position { public float x, y, z; }
public struct Velocity { public float x, y, z; }
public struct Rotation { public float x, y, z; }
public struct Test1 { }
public struct Test2 { }
public struct Test3 { }
public struct Test4 { }
public struct Test5 { }
public struct Test6 { }
public struct Test7 { }
public struct Test8 { }
public struct Test9 { }
public struct Test10 { }
public struct Test11 { }
public struct Test12 { }
