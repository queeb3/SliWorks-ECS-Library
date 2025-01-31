using SliLib.ECS;

class Program
{
    static void Main()
    {
        var conf = new Config()
        {
            TickIntervals = 1f,
            FrameRateCap = 1f
        };

        Nexus.Create(Config: conf);

        Nexus.Entity.Reg<Health>(1).Reg<Damage>(2);
        Nexus.Systems.Add(new SHp());

        EntityCreator.Hp();
        EntityCreator.Hp();
        EntityCreator.Hp();
        EntityCreator.Hp();
        EntityCreator.Hp();
        EntityCreator.Hp();
        EntityCreator.HpDmg();
        EntityCreator.HpDmg();
        EntityCreator.HpDmg();
        EntityCreator.HpDmg();

        Nexus.Self.Run();
    }
}

static class EntityCreator
{
    public static int Hp()
    {
        return Nexus.Entity
            .StageComp<Health>(new(100f, 3f))
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

struct Health(float max, float regenAmount, float regenTimer = 3f)
{
    public float Max = max;
    public float Current = max;
    public float RegenWait = regenTimer;
    public float RegenAmount = regenAmount;
}

struct Damage(float amount)
{
    public float Amount = amount;
}

class SHp : BaseSystem
{
    public override int Priority { get; } = 5;
    public override Query Query { get; } = new Query().With<Health>();

    float timer = 0;
    const float epsilon = 0.001f;

    public override void Update()
    {
        // query.Execute((arch, index) => Regen(arch, index));
    }

    public override void TickUpdate()
    {
        timer += Time.TickInterval;

        Query.Execute((arch, index) => Degen(arch, index));
    }

    void Regen(Archetype arch, int index)
    {
        ref Health hp = ref arch.GetComponent<Health>(index);

        if (Time.IsTickFrame() && MathF.Round(timer % hp.RegenWait, 4) < epsilon)
        {
            hp.Current = Math.Min(hp.Max, hp.Current + hp.RegenAmount);
            Console.WriteLine($"Archetype {arch.Id} Entity {index}: Health {hp.Current}/{hp.Max}");
        }
    }
    void Degen(Archetype arch, int index)
    {
        ref Health hp = ref arch.GetComponent<Health>(index);

        if (Time.IsTickFrame() && MathF.Round(timer % hp.RegenWait, 4) < epsilon)
        {
            hp.Current = Math.Max(0, hp.Current - hp.RegenAmount);
            Console.WriteLine($"Archetype {arch.Id} Entity {index}: Health {hp.Current}/{hp.Max}");
        }
    }
}
