namespace SliLib.ECS;

using SliLib.Tools.Debug;

public class Nexus
{
    private static Lazy<Nexus> instance = new(() => new Nexus());
    public static Nexus Self => instance.Value;

    Time time;
    Config config;

    EntityManager entity;
    SystemsList systems;

    private Nexus(Config? conf = null, EntityManager? entityManager = null, SystemsList? systemsList = null)
    {
        Log.General("Loading Nexus...");

        config = conf ?? new();
        time = new Time(config.TickIntervals, config.FrameRateCap);
        entity = entityManager ?? new();
        systems = systemsList ?? new();

        Log.General("Nexus Loaded!");
    }

    public static void Create(Config? Config = null, EntityManager? EntityManager = null, SystemsList? SystemsList = null)
    {
        instance = new(() => new(Config ?? new(), EntityManager ?? new(), SystemsList ?? new()));
    }

    public void Run()
    {
        var sys = systems.GetSystems();

        Log.General("Loading Loop in 5 seconds...");

        Thread.Sleep(5000); // simulate other initializations...

        Log.General("Starting Loop!");

        while (true)
        {
            Time.UpdateTime();

            foreach (var system in sys)
            {
                Log.General($"Calling for: {system.GetType().Name}.");

                system.Update();

                if (Time.IsTickFrame())
                    system.TickUpdate();
            }
        }
    }


    // ----------------------------- Singleton Accessors -----------------------------------
    public static Time Time => Self.time;
    public static Config Config => Self.config;
    public static EntityManager Entity => Self.entity;
    public static SystemsList Systems => Self.systems;
}
