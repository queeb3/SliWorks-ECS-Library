using System.Diagnostics;
using SliLib.ECS;

class Program
{
    public static void Main(string[] args)
    {
        /*
        what should i be testing about component registry?
        i need to ensure that when i create and register components they are stored non genericly
        without issue and can be retreived as a copy not a reference to their initially stored array.

        what about the speed of registration and retreival?
        those are not hot paths as they only occur on setup of the ecs at the start if the programs life,
        afterwards they are simple retrieved when requested and pushed into a componentset upon a new
        archetype being created.

        is this a realtime or runtime feature?
        no this is strictly for setup and the only runtime that could possibly happen is if a user/dev
        must get component meta data for testing their game/program. something that may be another consideration
        is archetype creation and how data is transfered from a register to a set; it may not be a hot path
        but it also shouldnt be a bottleneck and cause any performance issues when happening.

        what about memory layout?
        the layout for this is simple in register but slightly more complex for the Set only because
        the set must be data aligned via infos and components so the right meta data can be retreived.

        should meta info be stored in the set?
        this is something that needs testing to see if i should just go ahead and store a indexer by id to
        local index to provide a lookup indirection from a set to the register. the only thought being a
        reduction in overal size of an archetype so that it can maintain a 16kb size or slightly larger
        to maintain l1 and l2 cache locality when pulled for iteration.

        does component memory need to be any different?
        not unless it doesnt get a direct view into the reference of an array. the only concern i have
        with component memory lookups is that it will create a duplicate and not allow direct access
        to changing the internal components data at each index for a given type.
        */

        var sw = Stopwatch.StartNew();
        var compReg = new ComponentRegister();

        RegComponents(ref compReg);
        ComponentIdCheck(ref compReg);
        ComponentSizeCheck(ref compReg);
        ComponentCodeCheck(ref compReg);

        GetInfoTest(ref compReg);
        MaskAndTransferTest(ref compReg, out var mask);
        SetCreationTest(ref compReg, mask, out var set);
        SetArrayAccessTest(ref set);
        SetDataTransformTest(ref set, ref compReg);
        SetDataIntegrityTest(ref compReg);

        Console.WriteLine(sw.Elapsed);
        sw.Stop();
    }

    static void RegComponents(ref ComponentRegister reg)
    {
        reg.Reg<Player>().Reg<Npc>()
            .Reg<Position>().Reg<Velocity>()
            .Reg<Health>().Reg<Mana>()
            .Reg<Damage>().Reg<StatList>();

        Console.WriteLine("Register Finished!");
        Console.WriteLine($"Register: Capacity={reg.Capacity} | Count={reg.Count}");
    }

    static void ComponentIdCheck(ref ComponentRegister reg)
    {
        var pla = reg.GetInfo<Player>().Id;
        var npc = reg.GetInfo<Npc>().Id;
        var pos = reg.GetInfo<Position>().Id;
        var vel = reg.GetInfo<Velocity>().Id;
        var hea = reg.GetInfo<Health>().Id;
        var man = reg.GetInfo<Mana>().Id;
        var dmg = reg.GetInfo<Damage>().Id;
        var sta = reg.GetInfo<StatList>().Id;

        Console.WriteLine($"Id Check: Player={pla} | Npc={npc} | Position={pos} | Velocity={vel} | Health={hea} | Mana={man} | Damage={dmg} | Stats={sta}");
    }

    static void ComponentSizeCheck(ref ComponentRegister reg)
    {
        var pla = reg.GetInfo<Player>().Size;
        var npc = reg.GetInfo<Npc>().Size;
        var pos = reg.GetInfo<Position>().Size;
        var vel = reg.GetInfo<Velocity>().Size;
        var hea = reg.GetInfo<Health>().Size;
        var man = reg.GetInfo<Mana>().Size;
        var dmg = reg.GetInfo<Damage>().Size;
        var sta = reg.GetInfo<StatList>().Size;

        Console.WriteLine($"Size Check: Player={pla} | Npc={npc} | Position={pos} | Velocity={vel} | Health={hea} | Mana={man} | Damage={dmg} | Stats={sta}");
    }

    static void ComponentCodeCheck(ref ComponentRegister reg)
    {
        var pla = reg.GetInfo<Player>().Code;
        var npc = reg.GetInfo<Npc>().Code;
        var pos = reg.GetInfo<Position>().Code;
        var vel = reg.GetInfo<Velocity>().Code;
        var hea = reg.GetInfo<Health>().Code;
        var man = reg.GetInfo<Mana>().Code;
        var dmg = reg.GetInfo<Damage>().Code;
        var sta = reg.GetInfo<StatList>().Code;

        Console.WriteLine($"Code Check: Player={pla} | Npc={npc} | Position={pos} | Velocity={vel} | Health={hea} | Mana={man} | Damage={dmg} | Stats={sta}");
    }
    // will not need a type check because you have tpo be explicit in generic naming

    static void GetInfoTest(ref ComponentRegister reg)
    {
        var typeC = reg.GetInfo(typeof(Health));
        var genericC = reg.GetInfo<Health>();
        var idC = reg.GetInfo(4);
        var code = idC.Code;
        var codeC = reg.GetInfo(code); // kinda redundent but just wanna make sure it actually still getscorrect info

        Console.WriteLine($"GetInfoTest: TypeCheck={typeC.Type} | GenericCheck={genericC.Type} | IdCheck={idC.Type} | CodeCheck={codeC.Type}");
    }

    static void MaskAndTransferTest(ref ComponentRegister reg, out ChunkMask mask)
    {
        var cmb = new ChunkMaskBuilder(reg);
        var mask1 = cmb.Add<Position>().Add<Velocity>().Return();
        mask = mask1;

        var data1 = reg.GenerateTemplate(mask1);

        Console.WriteLine($"Mask-TransferTest: InfosIndex0={data1.Infos[0].Type} | InfosIndex1={data1.Infos[1].Type} || ArraysIndex0={data1.Arrays[0]} | ArraysIndex1={data1.Arrays[1]}");
    }

    static void SetCreationTest(ref ComponentRegister reg, ChunkMask mask, out ComponentSet set)
    {
        var data = reg.GenerateTemplate(mask);

        // set = reg.GenerateSetFromMask(mask);
        set = reg.GenerateSetFromTransfer(data);
        Console.WriteLine(set);
        Console.WriteLine($"Creating Set: Size={set.SizeOfSet} | Count={set.Count} | Mask={set.Mask}");
        Console.WriteLine(set.Mask.PrintChunkBits(0));
    }

    static void SetArrayAccessTest(ref ComponentSet set)
    {
        var pos = set.AccessArray<Position>();

        Console.WriteLine($"SetAccessTest: Grabbed={pos[0]} Array");
    }

    static void SetDataTransformTest(ref ComponentSet set, ref ComponentRegister reg)
    {
        var posA = set.AccessArray<Position>();
        // confirm it doesnt affect global registrar
        var posB = reg.DebugGrabArray<Position>();

        // set in this memory first
        posA[0].Values.X = 2.6f;
        posA[0].Values.Y = 56f;
        posA[0].Values.Z = 29999.13f;

        //access and read in new mem second
        var posA2 = set.AccessArray<Position>();

        posA2[0].Values.Y = 99.69f;

        Console.WriteLine($"SetTransformTest: SetCheck0X={posA2[0].Values.X} | SetCheck0Y={posA2[0].Values.Y} & V1={posA[0].Values.Y} | SetCheck0Z={posA2[0].Values.Z}");
        Console.WriteLine($"SetTransformTest: RegisterCheck0X={posB[0].Values.X} | RegisterCheck0Y={posB[0].Values.Y} | RegisterCheck0Z={posB[0].Values.Z}");
    }

    static void SetDataIntegrityTest(ref ComponentRegister reg)
    {
        var cmb = new ChunkMaskBuilder(reg);
        var mask = cmb.Add<Position>().Add<Velocity>().Add<Damage>().Return();
        var set = reg.GenerateSetFromMask(mask);
        var data = reg.GenerateTemplate(mask);
        var set2 = reg.GenerateSetFromTransfer(data);
        var set3 = reg.GenerateSetFromTransfer(data.Clone()); // confirms clone works
        var set4 = reg.GenerateSetFromTransfer(data);

        set.Edit<Damage>(0).Amount = 99;
        set2.Edit<Damage>(0).Amount = 2;
        set3.Edit<Damage>(0).Amount = 10;
        set4.Edit<Damage>(0).Amount = 15; // this will alter set2

        Console.WriteLine($"IntegrityTest: First={set.Edit<Damage>(0).Amount} | Second={set2.Edit<Damage>(0).Amount} | Third={set3.Edit<Damage>(0).Amount} | Fourth={set4.Edit<Damage>(0).Amount}");
    }

}

// test structs for registering
struct Player
{
    // identifier component
}

struct Npc
{
    // identifier component
}

struct DynamicStat // not a component
{
    public int Total;
    public int Current;
}

struct Health
{
    public DynamicStat Values;
}

struct Mana
{
    public DynamicStat Values;
}

struct Damage
{
    public float Amount;
}

struct StatList
{
    public float Stamina;
    public float Strength;
    public float Agility;
    public float Wisdom;
    public float Charisma;
}

struct XY // not a component
{
    public float X;
    public float Y;
}

struct XYZ // not a component
{
    public float X;
    public float Y;
    public float Z;
}

struct Position
{
    public XYZ Values;
}

struct Velocity
{
    public XYZ Values;
}
