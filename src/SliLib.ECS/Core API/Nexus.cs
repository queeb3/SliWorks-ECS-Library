namespace SliLib.ECS;

using SliLib.Tools.Debug;
/*
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
*/

public class Nexus
{
    public NexusRegistries Registries { get; private set; }

    // all must be true for run to operate
    // Create and CreateDefault will set all to true
    private bool entBound = false;
    private bool compBound = false;
    private bool archBound = false;
    private bool queryBound = false;
    private bool optionsLoaded = false;
    private bool AllClear() => entBound && compBound && archBound && queryBound && optionsLoaded;

    private Nexus(NexusRegistries registries)
    {
        Registries = registries;
    }

    private Nexus() { } // for create api

    public static Nexus CreateDefault()
    {
        return new()
        {
            Registries = new NexusRegistries() with
            {
                // TODO fill in
            },

            entBound = true,
            compBound = true,
            archBound = true,
            queryBound = true,
            optionsLoaded = true
        };
    }

    public static Nexus Create()
    {
        return new();
    }

    // this is psuedo till i recreate all the needed classes, should be easy since i already did it once right???
    public void RunLoop()
    {
        // TODO create run loop
    }

    public void RunStepFrame()
    {
        // TODO to be used inside a custom loop steps through everything
        // for 1 frame and waits for next call
    }

    public void BindEntityRegister(EntityRegister register)
    {
        entBound = true;
        Registries.Entity = register;
    }

    public void BindComponentRegister(ComponentRegister register)
    {
        compBound = true;
        Registries.Component = register;
    }

    public void BindArchetypeRegister(/*insert class*/)
    {
        archBound = true;
        // TODO add archetype to registries
    }

    public void BindQueryManager(/*insert class*/)
    {
        queryBound = true;
        // TODO add query to registries
    }

    public void BindSystem()
    {
        // TODO add based on priority
    }

    public void BindSystems()
    {

    }

    public void BindSystemChain()
    {
        // TODO first system is added based on priority and is the leader of
        // the chain and each after must be executed in order
    }

    // TODO figure out how i want to do system events and conditionals will decide later
    // probably going to need to somehow assign Action or Func and check if its ever called
    // and if so call the system attatched to that method.
    // how will i manage which system communicate if done by events?
    // how will i let systems know when to execute if a method is invoked?
    // do i need a action/func manager with systems attached as senders and then attach subscribers?
    // much to think!

    public void BindComponents(params Type[] components)
    {
        // TODO it might be beneficial to do a simple first time reflection to find all components to register them?
        // otherwise manual calls for each T component with method chaining
        // needs testing, but im sure it wont be a problem to do reflections for just loading initially
    }
}
