namespace SliLib.ECS;

public class EntityEditor
{
    private ArchetypeRegistry ar;

    public EntityEditor(ArchetypeRegistry ar)
    {
        this.ar = ar;
    }

    /// <summary>
    /// NOTE: Only works on entities with this component and that are already in the register.
    /// <br/><br/>
    /// Allows direct component editing for a entity.
    /// </summary>
    /// <typeparam name="T">Component to edit.</typeparam>
    /// <param name="entity">Id to edit. make sure its correct.</param>
    /// <returns>A direct reference to the data stored in memory.</returns>
    public ref T Edit<T>(EntityInfo entity) where T : struct
    {
        return ref ar.GetEntityArchetype(entity).Get<T>(entity);
    }

    /// <summary>
    /// NOTE: Only works on entities with this component.
    /// <br/><br/>
    /// Sets new data for the specified component.
    /// </summary>
    /// <typeparam name="T">Component to set.</typeparam>
    /// <param name="data">Overriding value.</param>
    /// <param name="entity">Id to edit. make sure its correct.</param>
    public void Set<T>(T data, EntityInfo entity) where T : struct
    {
        Edit<T>(entity) = data;
    }

    /// <summary>
    /// NOTE: Only works on already added entities.
    /// <br/><br/>
    /// Gets the chunk an entity is located in.
    /// </summary>
    /// <returns><see cref="Chunk"/> that the entity is located in.</returns>
    public Chunk GetEntityChunk(int entity)
    {
        var info = ar.GetEntityInfo(entity);
        ar.GetEntityArchetype(ar.GetEntityInfo(entity)).TryGetChunk(info.ChunkIndex, out var chunk);
        return chunk;
    }
}
