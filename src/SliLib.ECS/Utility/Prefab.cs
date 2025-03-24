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
        ar.GetEntityArchetype(info).Get<T>(info) = Data;
        return info;
    }
}
public class Prefab<T, T1> where T : struct where T1 : struct
{
    public T Data;
    public T1 Data1;

    public Prefab(T data, T1 data1)
    {
        Data = data;
        Data1 = data1;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>().Add<T1>();
        var info = eb.InsertToArchetype(ar);
        var arch = ar.GetEntityArchetype(info);

        arch.Get<T>(info) = Data;
        arch.Get<T1>(info) = Data1;

        return info;
    }
}
public class Prefab<T, T1, T2> where T : struct where T1 : struct where T2 : struct
{
    public T Data;
    public T1 Data1;
    public T2 Data2;

    public Prefab(T data, T1 data1, T2 data2)
    {
        Data = data;
        Data1 = data1;
        Data2 = data2;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>().Add<T1>().Add<T2>();
        var info = eb.InsertToArchetype(ar);
        var arch = ar.GetEntityArchetype(info);

        arch.Get<T>(info) = Data;
        arch.Get<T1>(info) = Data1;
        arch.Get<T2>(info) = Data2;

        return info;
    }
}
public class Prefab<T, T1, T2, T3> where T : struct where T1 : struct where T2 : struct where T3 : struct
{
    public T Data;
    public T1 Data1;
    public T2 Data2;
    public T3 Data3;

    public Prefab(T data, T1 data1, T2 data2, T3 data3)
    {
        Data = data;
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>().Add<T1>().Add<T2>().Add<T3>();
        var info = eb.InsertToArchetype(ar);
        var arch = ar.GetEntityArchetype(info);

        arch.Get<T>(info) = Data;
        arch.Get<T1>(info) = Data1;
        arch.Get<T2>(info) = Data2;
        arch.Get<T3>(info) = Data3;

        return info;
    }
}
public class Prefab<T, T1, T2, T3, T4> where T : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct
{
    public T Data;
    public T1 Data1;
    public T2 Data2;
    public T3 Data3;
    public T4 Data4;

    public Prefab(T data, T1 data1, T2 data2, T3 data3, T4 data4)
    {
        Data = data;
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>().Add<T1>().Add<T2>().Add<T3>().Add<T4>();
        var info = eb.InsertToArchetype(ar);
        var arch = ar.GetEntityArchetype(info);

        arch.Get<T>(info) = Data;
        arch.Get<T1>(info) = Data1;
        arch.Get<T2>(info) = Data2;
        arch.Get<T3>(info) = Data3;
        arch.Get<T4>(info) = Data4;

        return info;
    }
}
public class Prefab<T, T1, T2, T3, T4, T5> where T : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct
{
    public T Data;
    public T1 Data1;
    public T2 Data2;
    public T3 Data3;
    public T4 Data4;
    public T5 Data5;

    public Prefab(T data, T1 data1, T2 data2, T3 data3, T4 data4, T5 data5)
    {
        Data = data;
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>().Add<T1>().Add<T2>().Add<T3>().Add<T4>().Add<T5>();
        var info = eb.InsertToArchetype(ar);
        var arch = ar.GetEntityArchetype(info);

        arch.Get<T>(info) = Data;
        arch.Get<T1>(info) = Data1;
        arch.Get<T2>(info) = Data2;
        arch.Get<T3>(info) = Data3;
        arch.Get<T4>(info) = Data4;
        arch.Get<T5>(info) = Data5;

        return info;
    }
}
public class Prefab<T, T1, T2, T3, T4, T5, T6> where T : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct
{
    public T Data;
    public T1 Data1;
    public T2 Data2;
    public T3 Data3;
    public T4 Data4;
    public T5 Data5;
    public T6 Data6;

    public Prefab(T data, T1 data1, T2 data2, T3 data3, T4 data4, T5 data5, T6 data6)
    {
        Data = data;
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
        Data6 = data6;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>().Add<T1>().Add<T2>().Add<T3>().Add<T4>().Add<T5>().Add<T6>();
        var info = eb.InsertToArchetype(ar);
        var arch = ar.GetEntityArchetype(info);

        arch.Get<T>(info) = Data;
        arch.Get<T1>(info) = Data1;
        arch.Get<T2>(info) = Data2;
        arch.Get<T3>(info) = Data3;
        arch.Get<T4>(info) = Data4;
        arch.Get<T5>(info) = Data5;
        arch.Get<T6>(info) = Data6;

        return info;
    }
}
public class Prefab<T, T1, T2, T3, T4, T5, T6, T7> where T : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct
{
    public T Data;
    public T1 Data1;
    public T2 Data2;
    public T3 Data3;
    public T4 Data4;
    public T5 Data5;
    public T6 Data6;
    public T7 Data7;

    public Prefab(T data, T1 data1, T2 data2, T3 data3, T4 data4, T5 data5, T6 data6, T7 data7)
    {
        Data = data;
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
        Data6 = data6;
        Data7 = data7;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>().Add<T1>().Add<T2>().Add<T3>().Add<T4>().Add<T5>().Add<T6>().Add<T7>();
        var info = eb.InsertToArchetype(ar);
        var arch = ar.GetEntityArchetype(info);

        arch.Get<T>(info) = Data;
        arch.Get<T1>(info) = Data1;
        arch.Get<T2>(info) = Data2;
        arch.Get<T3>(info) = Data3;
        arch.Get<T4>(info) = Data4;
        arch.Get<T5>(info) = Data5;
        arch.Get<T6>(info) = Data6;

        return info;
    }
}
public class Prefab<T, T1, T2, T3, T4, T5, T6, T7, T8> where T : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct where T8 : struct
{
    public T Data;
    public T1 Data1;
    public T2 Data2;
    public T3 Data3;
    public T4 Data4;
    public T5 Data5;
    public T6 Data6;
    public T7 Data7;
    public T8 Data8;

    public Prefab(T data, T1 data1, T2 data2, T3 data3, T4 data4, T5 data5, T6 data6, T7 data7, T8 data8)
    {
        Data = data;
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
        Data6 = data6;
        Data7 = data7;
        Data8 = data8;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>().Add<T1>().Add<T2>().Add<T3>().Add<T4>().Add<T5>().Add<T6>().Add<T7>().Add<T8>();
        var info = eb.InsertToArchetype(ar);
        var arch = ar.GetEntityArchetype(info);

        arch.Get<T>(info) = Data;
        arch.Get<T1>(info) = Data1;
        arch.Get<T2>(info) = Data2;
        arch.Get<T3>(info) = Data3;
        arch.Get<T4>(info) = Data4;
        arch.Get<T5>(info) = Data5;
        arch.Get<T6>(info) = Data6;
        arch.Get<T7>(info) = Data7;
        arch.Get<T8>(info) = Data8;

        return info;
    }
}
public class Prefab<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> where T : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct where T8 : struct where T9 : struct
{
    public T Data;
    public T1 Data1;
    public T2 Data2;
    public T3 Data3;
    public T4 Data4;
    public T5 Data5;
    public T6 Data6;
    public T7 Data7;
    public T8 Data8;
    public T9 Data9;

    public Prefab(T data, T1 data1, T2 data2, T3 data3, T4 data4, T5 data5, T6 data6, T7 data7, T8 data8, T9 data9)
    {
        Data = data;
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
        Data6 = data6;
        Data7 = data7;
        Data8 = data8;
        Data9 = data9;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>().Add<T1>().Add<T2>().Add<T3>().Add<T4>().Add<T5>().Add<T6>().Add<T7>().Add<T8>().Add<T9>();
        var info = eb.InsertToArchetype(ar);
        var arch = ar.GetEntityArchetype(info);

        arch.Get<T>(info) = Data;
        arch.Get<T1>(info) = Data1;
        arch.Get<T2>(info) = Data2;
        arch.Get<T3>(info) = Data3;
        arch.Get<T4>(info) = Data4;
        arch.Get<T5>(info) = Data5;
        arch.Get<T6>(info) = Data6;
        arch.Get<T7>(info) = Data7;
        arch.Get<T8>(info) = Data8;
        arch.Get<T9>(info) = Data9;

        return info;
    }
}
public class Prefab<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> where T : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct where T8 : struct where T9 : struct where T10 : struct
{
    public T Data;
    public T1 Data1;
    public T2 Data2;
    public T3 Data3;
    public T4 Data4;
    public T5 Data5;
    public T6 Data6;
    public T7 Data7;
    public T8 Data8;
    public T9 Data9;
    public T10 Data10;

    public Prefab(T data, T1 data1, T2 data2, T3 data3, T4 data4, T5 data5, T6 data6, T7 data7, T8 data8, T9 data9, T10 data10)
    {
        Data = data;
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
        Data6 = data6;
        Data7 = data7;
        Data8 = data8;
        Data9 = data9;
        Data10 = data10;
    }

    public EntityInfo UseFab(EntityBuilder eb, ArchetypeRegistry ar)
    {
        eb.Add<T>().Add<T1>().Add<T2>().Add<T3>().Add<T4>().Add<T5>().Add<T6>().Add<T7>().Add<T8>().Add<T9>().Add<T10>();
        var info = eb.InsertToArchetype(ar);
        var arch = ar.GetEntityArchetype(info);

        arch.Get<T>(info) = Data;
        arch.Get<T1>(info) = Data1;
        arch.Get<T2>(info) = Data2;
        arch.Get<T3>(info) = Data3;
        arch.Get<T4>(info) = Data4;
        arch.Get<T5>(info) = Data5;
        arch.Get<T6>(info) = Data6;
        arch.Get<T7>(info) = Data7;
        arch.Get<T8>(info) = Data8;
        arch.Get<T9>(info) = Data9;
        arch.Get<T10>(info) = Data10;

        return info;
    }
}
