using System.Reflection;

namespace SliLib.ECS;

public interface IComponentArray // still needed but only for storing and casting
{
    public int AddDefault();
    public void SetDefault(int index);
    public object GetBoxData(int index);
    public void SetBoxAtIndex(int index, object box);
    public bool Remove(int index);
    public int Count { get; }
}
public interface IGenericArray<T> : IComponentArray // remove
{
    public ref T Ref(int index);
    public int Add(T item);
}


/// <summary>
/// Array of structs.</br>
/// Stores components for access by ref at their index.
/// </summary>
/// <typeparam name="T">Component struct</typeparam>
public class ComponentArray<T> : IComponentArray, IGenericArray<T> where T : struct
{
    private T[] components; // convert to a single instance of T
    private Stack<int> openSlots; // remove
    private int count; // remove
    private int capacity; // replace

    public int Count => count;
    public int Capacity => capacity;
    public int Free => capacity - count;

    public ComponentArray(int initialCapacity = 16) // redesign
    {
        capacity = initialCapacity > 0 ? initialCapacity : throw new ArgumentException("Capacity must be greater than zero.");
        components = new T[capacity]; // change to allow capacity
        openSlots = new Stack<int>();
        count = 0;
    }
    public ComponentArray()
    {
        capacity = 16 > 0 ? 16 : throw new ArgumentException("Capacity must be greater than zero.");
        components = new T[capacity];
        openSlots = new Stack<int>();
        count = 0;
    }

    public T this[int index]
    {
        get => Ref(index);
        set => components[index] = value;
    }

    void IComponentArray.SetDefault(int index) // remove
    {
        this[index] = default;
    }
    int IComponentArray.AddDefault() => Add(default); // remove
    object IComponentArray.GetBoxData(int index) => BoxData(index); // remove
    void IComponentArray.SetBoxAtIndex(int index, object box) => Unbox(index, box); // remove

    public int Add(T component) // remove in favor of direct index access for changing fields
    {
        int index;
        if (openSlots.Count > 0)
        {
            index = openSlots.Pop();
        }
        else
        {
            index = count;
            if (count >= capacity)
                Resize();
        }

        components[index] = component;
        count++;
        return index;
    }

    public bool Remove(int index) // same as add
    {
        if (!ValidateIndex(index, throwIfInvalid: false))
            return false;

        components[index] = default;
        openSlots.Push(index);
        count--;

        return true;
    }

    public ref T Ref(int index) // alter to just get storage for direct access of indexed fields
    {
        if (!ValidateIndex(index, throwIfInvalid: true))
        {
            throw new IndexOutOfRangeException($"Invalid index {index} for ComponentArray<{typeof(T).Name}>.");
        }

        return ref components[index];
    }

    private bool ValidateIndex(int index, bool throwIfInvalid = true) // remove will no longer be needed
    {
        bool isValid = index >= 0 && index < capacity && !openSlots.Contains(index);
        if (!isValid && throwIfInvalid)
        {
            throw new IndexOutOfRangeException($"Invalid index {index} for ComponentArray<{typeof(T).Name}>.");
        }
        return isValid;
    }

    private object BoxData(int index) // remove
    {
        if (!ValidateIndex(index, throwIfInvalid: true))
            throw new IndexOutOfRangeException($"Invalid index {index} for ComponentArray<{typeof(T).Name}>.");

        return components[index];
    }

    private void Unbox(int index, object box) // remove
    {
        if (!ValidateIndex(index, throwIfInvalid: true))
            throw new IndexOutOfRangeException($"Invalid index {index} for ComponentArray<{typeof(T).Name}>.");

        if (box is T t)
        {
            this[index] = t;
        }
        else
        {
            throw new InvalidCastException($"The provided object cannot be cast to {typeof(T).Name}.");
        }
    }

    private void Resize() // remove in favor of direct memory size allocation, create dodcumentation to let people know to be careful with capacity allocations
    {
        if (capacity < 1024) capacity *= 2;
        else if (capacity <= int.MaxValue) capacity += 256;
        else throw new InvalidOperationException("Capacity limit has been reached.");

        Array.Resize(ref components, capacity);
    }
}
