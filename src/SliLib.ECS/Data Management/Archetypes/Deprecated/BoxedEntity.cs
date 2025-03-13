// namespace SliLib.ECS;

// public class BoxedEnt
// {
//     public int GlobalIndex; // entity array index
//     private int localIndex; // old archetype index
//     private Dictionary<Type, object> boxedComps;


//     public BoxedEnt(int global, int local)
//     {
//         GlobalIndex = global;
//         localIndex = local;
//         boxedComps = [];
//     }

//     public void BoxArchComps(Archetype arch)
//     {
//         foreach (var (type, array) in arch.GetArrays())
//         {
//             boxedComps.Add(type, array.GetBoxData(localIndex));
//         }
//     }

//     public object GetBoxedComponent(Type type)
//     {
//         if (!boxedComps.TryGetValue(type, out var component))
//             throw new KeyNotFoundException($"Component of type {type.Name} is not boxed for this entity.");
//         return component;
//     }

//     public bool BoxComponent(Type type, object component)
//     {
//         return boxedComps.TryAdd(type, component);
//     }

//     public bool Contains(Type type) => boxedComps.ContainsKey(type);
//     public IEnumerable<Type> GetBoxedTypes() => boxedComps.Keys;
// }
