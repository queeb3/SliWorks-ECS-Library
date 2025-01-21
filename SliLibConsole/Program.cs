using SliLib;
using SliLib.Query;
using SliLib.Archetypes;
using SliLib.Systems;

class Program
{
    static void Main()
    {
        Nexus.Entity.Reg<Health>(1).Reg<Damage>(2);
        Nexus.Systems.Add([new SWorld()]);
        Nexus.Config.TickIntervals = 1f;
        Nexus.Config.FrameRateCap = 1f;
        Nexus.Self.Run();
    }
}

static class EntityCreator
{
    public static int Hp()
    {
        return Nexus.Entity
            .StageComp<Health>(new(100f, 1f))
            .CreateEnt();
    }
    public static int HpDmg()
    {
        return Nexus.Entity
            .StageComp<Health>(new(100f, 1f))
            .StageComp<Damage>(new(5f))
            .CreateEnt();
    }
}


struct Health(float max, float regenTimer = 3f)
{
    public float Max = max;
    public float Current = max;
    public float RegenWait = regenTimer;
}

struct Damage(float amount)
{
    public float Amount = amount;
}

class SHp : BaseSystem
{
    public override int Priority { get; } = 5;
    public override Query Query { get; } = new Query().With<Health>();

    public float timer = 0f;

    public override void Update()
    {
        // query.Execute((arch, index) => Regen(arch, index));
    }

    public override void TickUpdate()
    {
        timer += Time.TickInterval;

        Query.Execute((arch, index) => Regen(arch, index));
    }

    void Regen(Archetype arch, int index)
    {
        ref Health hp = ref arch.GetComponent<Health>(index);

        if (Time.IsTickFrame && timer % hp.RegenWait == 1)
        {
            hp.Current = Math.Min(hp.Max, hp.Current + 0.5f);
            Console.WriteLine($"Archetype {arch.Id} Entity {index}: Health {hp.Current}/{hp.Max}");
        }
    }
}

class SCollider : BaseSystem
{
    public override int Priority { get; }
    public override Query Query { get; } = new Query();

    public override void Update()
    {

    }
}
