using System.Diagnostics;
using SliLib.ECS;
class Program
{
    /*
    NOTE: this is a very extreme stress test when doing the loops to get a good understandin of what the
    entity register class is capable of. i know there could possibly be more things to test but i feel
    that this is pretty reasonable for what it can do in such small amounts of time.
    */

    public static void Main(string[] args)
    {
        var ents = new EntityRegister(capacity: 16, resizable: true);

        Console.WriteLine($"start: ecount={ents.Count}, bcount={ents.BCount}, cap={ents.Capacity}, resizable={ents.Resizable}");

        // InfoTest(ref ents);
        // CreateTest(ref ents);
        // DestroyTest(ref ents);
        LoopCreateTest(ref ents, 512 * 1_000_000); // 512 mil test
        // LoopCreateDestroyTest(ref ents, 512 * 100000); // 50 mil test
    }

    static void RegState(ref EntityRegister reg, int capOverwrite = 16)
    {
        Console.WriteLine($"RegState: {reg.Count} | {reg.BCount} | {reg.Capacity}");
        reg = new(capOverwrite, true); // reset for next tests
    }

    static void CreateTest(ref EntityRegister reg) // see if i can create a fresh entity
    {
        var id = reg.Create(out var info);
        Console.WriteLine($"CreateTest: entity {id} was created.");
        RegState(ref reg);
    }

    static void InfoTest(ref EntityRegister reg)
    {
        var b = reg.GenerateInfoFromId(0, out var info);
        Console.WriteLine($"InfoTest: infoID={info.Id} | was valid? {b}");
        b = reg.GenerateInfoFromId(-10, out info);
        Console.WriteLine($"InfoTest: infoID={info.Id} | was valid? {b}");
        b = reg.GenerateInfoFromId(20, out info);
        Console.WriteLine($"InfoTest: infoID={info.Id} | was valid? {b}");
        b = reg.GenerateInfoFromId(9999999, out info);
        Console.WriteLine($"InfoTest: infoID={info.Id} | was valid? {b}");

        var id = reg.Create(out var ei);
        b = reg.GenerateInfoFromId(id, out info);
        Console.WriteLine($"InfoTest: infoID={info.Id} | was valid? {b}");
        reg.Destroy(id);
        b = reg.GenerateInfoFromId(id, out info);
        Console.WriteLine($"InfoTest: infoID={info.Id} | was valid? {b}");
        RegState(ref reg);
    }

    static void LoopCreateTest(ref EntityRegister reg, int runs)
    {
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < runs; i++)
        {
            reg.Create(out var info);
        }

        Console.WriteLine($"LoopCreateTest: count={reg.Count} | block={reg.BCount} | time={sw.Elapsed}");
        RegState(ref reg);
    }

    static void DestroyTest(ref EntityRegister reg)
    {
        Console.WriteLine("starting destroy test, it doesnt cause writelines normally");
        reg.Destroy(0);
        reg.Destroy(-1);
        reg.Destroy(-100);
        reg.Destroy(100);
        reg.Destroy(99999999);
        RegState(ref reg, 50000);
    }

    static void LoopCreateDestroyTest(ref EntityRegister reg, int runs)
    {
        var ctr = 0;
        var missTracker = 0;
        var rnd = new Random();
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < runs; i++)
        {
            ctr++;
            missTracker++;
            reg.Create(out var info);
            if (ctr == 16) // 320000 destroys roughly
            {
                reg.Destroy(rnd.Next(0, i));
                ctr = 0;
                if (i >= runs / 2) ctr = 17;
            }

            if (missTracker >= 500_000)
            {
                missTracker = 0;
                Console.WriteLine($"CDTest: Unfilled={reg.DebugMC()} | MissingBits={reg.DebugMCB()} | Blocks={reg.BCount} | Ents={reg.Count} | TimeElapsed={sw.Elapsed}");
            }
        }

        Console.WriteLine($"LoopCDTest: count={reg.Count} | block={reg.BCount} | time={sw.Elapsed} | missing={reg.DebugMC()} | totalMissingEntries={reg.DebugMCB()}");

        // RegState(ref reg);
    }
}
