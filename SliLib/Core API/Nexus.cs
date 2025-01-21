namespace SliLib;

using SliLib.Core;
using SliLib.Managers;
using SliLib.Systems;

public class Nexus
{
    private static readonly Lazy<Nexus> instance = new(() => new Nexus());
    public static Nexus Self => instance.Value;

    Time time { get; }
    Config config { get; }

    EntityManager entity { get; }
    SystemsList systems { get; }

    private Nexus(Config? conf = null, EntityManager? entityManager = null, SystemsList? systemsList = null)
    {
        Console.WriteLine("Initializing Nexus...");

        config = conf ?? new Config();
        time = new Time(config);
        entity = entityManager ?? new EntityManager();
        systems = systemsList ?? new SystemsList();

        Console.WriteLine("Finished...");
    }

    public void Run()
    {
        var sys = systems.GetSystems();

        Console.WriteLine("Loading Loop in 10 seconds...");

        Thread.Sleep(10000);

        Console.WriteLine("Sarting Loop...");

        while (true)
        {
            Time.UpdateTime();

            foreach (var system in sys)
            {
                system.Update();

                if (Time.IsTickFrame)
                    system.TickUpdate();
            }
        }
    }


    // ----------------------------- Singletone Accessors -----------------------------------
    public static Time Time => Self.time;
    public static Config Config => Self.config;
    public static EntityManager Entity => Self.entity;
    public static SystemsList Systems => Self.systems;
}
