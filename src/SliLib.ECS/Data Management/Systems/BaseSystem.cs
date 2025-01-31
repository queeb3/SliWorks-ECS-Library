namespace SliLib.ECS;

public abstract class BaseSystem
{
    public static Time Time => Nexus.Time;

    public abstract int Priority { get; } // higher numbers are ran last lower numbers are ran first
    public abstract Query Query { get; }
    public abstract void Update(); // every frame - required to override
    public virtual void TickUpdate() { return; } // called only on tick frames - not required to override

    // TODO conditional func, custom event, chaining
}
