using System.Diagnostics;

namespace SliLib.ECS;

// these all allow for creating a entity prefab up to a certain limit of components to make it easier to mass spawn
// low cost entities, entities that require more depth will still need to be manually allocated but can be done using
// custom hand made factories you make yourself.

public class Prefab<T> where T : struct
{
    public T Data; // preconfigured data

    public Prefab(T data)
    {
        Data = data;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>();
        var info = eb.InsertToArchetype(ar);
        ar.GetEntityArchetype(info.Id).Get<T>(info) = Data;
        return info;
    }
}
