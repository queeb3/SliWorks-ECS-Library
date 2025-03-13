// namespace SliLib.ECS;
// public class EntityArray
// {

//     private bool[] ids;
//     private Stack<int> free;

//     public int ActiveCount { get; private set; }
//     public int Expansions { get; private set; }
//     public int Capacity { get; private set; }


//     public EntityArray(int capacity = 500)
//     {
//         Capacity = capacity;

//         ids = new bool[Capacity];
//         free = new Stack<int>(Capacity);

//         // Add new IDs in reverse order, starting from the last new ID to the first
//         for (int i = Capacity - 1; i >= 0; i--)
//         {
//             free.Push(i);
//         }

//         ActiveCount = 0;
//         Expansions = 1; // initial expansion
//     }

//     public bool this[int index]
//     {
//         get => Contains(index);
//     }

//     public int Add()
//     {
//         if (free.Count == 0)
//         {
//             Expansions++;
//             int newCapacity = Capacity * Expansions;


//             // Prevent adding IDs already in use from previous expansions
//             for (int i = newCapacity - 1; i >= Capacity * (Expansions - 1); i--)
//             {
//                 free.Push(i);
//             }

//             Resize(newCapacity);
//             Capacity = newCapacity;
//         }

//         var ent = free.Pop();
//         ids[ent] = true;

//         ActiveCount++;
//         return ent;
//     }

//     public bool Remove(int ent)
//     {
//         if (!Contains(ent)) return false;

//         ids[ent] = false;
//         free.Push(ent);
//         ActiveCount--;

//         return true;
//     }

//     public bool Contains(int ent)
//     {
//         return ent >= 0 && ent < ids.Length && ids[ent];
//     }

//     private void Resize(int amount)
//     {
//         Array.Resize(ref ids, amount);
//     }
// }
