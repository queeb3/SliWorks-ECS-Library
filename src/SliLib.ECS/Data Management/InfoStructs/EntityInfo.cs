namespace SliLib.ECS;

/// <summary>
/// Important info card that gives meta data about an entity.
/// <br/><br/>
/// NOTE: Does not always mean the entity exists, to check if entity exists please utilize <see cref="EntityRegister"/>.
/// </summary>
public record EntityInfo
{
    /// <summary>
    /// The system wide recognized <c>Id</c> for this entity.
    /// <br/><br/>
    /// <c>Id</c> is the direct index to the bit in <see cref="EntityRegister"/>.
    /// </summary>
    public int Id { get; }
    /// <summary>
    /// <c>Id</c> for the <see cref="Archetype"/> that this entity is currenly stored in.
    /// <br/><br/>
    /// This is also the <c>Index</c> where the <see cref="Archetype"/> is stored in <see cref= "ArchetypeRegister"/>.
    /// </summary>
    public int ArchetypeId { get; internal set; }
    /// <summary>
    /// The actual index of the Chunk inside the <see cref="Archetype"/> containing this entity's <c>Id</c>.
    /// </summary>
    public int ChunkIndex { get; internal set; }
    /// <summary>
    /// Internal <c>Id</c> of the archetype pre calculation of where the entity is located.
    /// <br/><br/>
    /// NOTE: the id is not an exact index it must go through 2 calculations.
    /// <br/>
    /// chunk index = id / arraylength -> local index = id % arraylength.
    /// </summary>
    public int LocalId { get; internal set; }

    public EntityInfo(int id)
    {
        Id = id;
        ArchetypeId = -1;
        ChunkIndex = -1;
        LocalId = -1;
    }
}
