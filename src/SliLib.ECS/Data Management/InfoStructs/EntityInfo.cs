namespace SliLib.ECS;

public struct EntityInfo
{
    public int Id { get; }
    public short ArchetypeId { get; private set; } // Unique archetype reference
    public short ArchIndex { get; private set; } // Local index within archetype

    public EntityInfo(int id)
    {
        Id = id;
        ArchetypeId = -1; // Unset
        ArchIndex = -1;
    }

    public void SetArch(short archetypeId, short archIndex)
    {
        if (ArchetypeId != -1)
            throw new InvalidOperationException($"Entity {Id} is already assigned to an archetype!");

        ArchetypeId = archetypeId;
        ArchIndex = archIndex;
    }

    public void MoveEntity(short newArchetypeId, short newArchIndex)
    {
        if (ArchetypeId == -1)
            throw new InvalidOperationException($"Entity {Id} has no archetype to move from!");

        ArchetypeId = newArchetypeId;
        ArchIndex = newArchIndex;
    }
}
