namespace SliLib.ECS;

/// <summary>
/// Meta card for everything related to a specific <see cref="Archetype"/>.
/// <br/><br/>
/// Holds: Index as Id | Instance
/// </summary>
public record ArchInfo
{
    /// <summary>
    /// unique Id for every <see cref="Archetype"/> which also acts as its indexer.
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// Instance of the <see cref="Archetype"/>.
    /// </summary>
    public Archetype Instance { get; init; }

    public ArchInfo(int id, ComponentSetTemplate template)
    {
        Id = id;
        Instance = new Archetype(Id, template);
    }
}
