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

        var compReg = new ComponentRegister();

    }
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

struct Item // not a component
{
    public int MaxStack;
    public int Amount;
    public int Type;
    public int Variant;
}

struct XY // not a component
{
    public float X, Y;
}

struct Block
{
    public Item DropInfo;
    public XY Position;
}
