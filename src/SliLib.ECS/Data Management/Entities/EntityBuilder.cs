namespace SliLib.ECS;

/// <summary>
/// Allows direct insertion and creation of entities for a <see cref="ArchetypeRegistry"/>.
/// </summary>
public class EntityBuilder
{
    private EntityRegister er;
    private ChunkMaskBuilder mb;

    public EntityBuilder(EntityRegister entityRegister, ComponentRegister componentRegister)
    {
        er = entityRegister;
        mb = new(componentRegister);
    }

    /// <summary>
    /// Easy chain for adding components
    /// </summary>
    public EntityBuilder Add<T>() where T : struct
    {
        mb.Add<T>();
        return this;
    }

    /// <summary>
    /// Adds the entity directly to a specified <see cref="ArchetypeRegistry"/>.
    /// </summary>
    /// <param name="archetypeRegistry">Register to be added too.</param>
    /// <returns>The generated <see cref="EntityInfo"/></returns>
    public EntityInfo InsertToArchetype(ArchetypeRegistry archetypeRegistry)
    {
        er.Create(out var entity);
        var mask = mb.Return();

        entity = archetypeRegistry.AddEntity(entity, mask);
        return entity;
    }
}
