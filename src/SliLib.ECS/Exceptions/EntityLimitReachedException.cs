namespace SliLib.ECS;

using System;

public class EntityLimitReachedException : Exception
{
    public EntityLimitReachedException()
        : base("Entity limit has been reached.") { }

    public EntityLimitReachedException(string message)
        : base(message) { }

    public EntityLimitReachedException(string message, Exception innerException)
        : base(message, innerException) { }
}
