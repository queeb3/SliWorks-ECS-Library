namespace SliLib.ECS;

using System;

public class EntityArchReplaceDontSetException : Exception
{
    public EntityArchReplaceDontSetException(int id)
        : base($"Entity {id} already has a set Archetype please call MoveEntity().") { }

    public EntityArchReplaceDontSetException(string message)
        : base(message) { }

    public EntityArchReplaceDontSetException(string message, Exception innerException)
        : base(message, innerException) { }
}
