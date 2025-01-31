namespace SliLib.ECS;

public interface IComponentArray
{
    public int AddDefault();
    public void SetDefault(int index);
    public object GetBoxData(int index);
    public void SetBoxAtIndex(int index, object box);
    public bool Remove(int index);
    public int Count { get; }
}
public interface IGenericArray<T> : IComponentArray
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
    private T[] components;
    private Stack<int> openSlots;
    private int count;
    private int capacity;

    public int Count => count;
    public int Capacity => capacity;
    public int Free => capacity - count;

    public ComponentArray(int initialCapacity = 16)
    {
        capacity = initialCapacity > 0 ? initialCapacity : throw new ArgumentException("Capacity must be greater than zero.");
        components = new T[capacity];
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

    void IComponentArray.SetDefault(int index)
    {
        this[index] = default;
    }
    int IComponentArray.AddDefault() => Add(default);
    object IComponentArray.GetBoxData(int index) => BoxData(index);
    void IComponentArray.SetBoxAtIndex(int index, object box) => Unbox(index, box);

    public int Add(T component)
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

    public bool Remove(int index)
    {
        if (!ValidateIndex(index, throwIfInvalid: false))
            return false;

        components[index] = default;
        openSlots.Push(index);
        count--;

        return true;
    }

    public ref T Ref(int index) // can cause a crash
    {
        if (!ValidateIndex(index, throwIfInvalid: true))
        {
            throw new IndexOutOfRangeException($"Invalid index {index} for ComponentArray<{typeof(T).Name}>.");
        }

        return ref components[index];
    }

    private bool ValidateIndex(int index, bool throwIfInvalid = true)
    {
        bool isValid = index >= 0 && index < capacity && !openSlots.Contains(index);
        if (!isValid && throwIfInvalid)
        {
            throw new IndexOutOfRangeException($"Invalid index {index} for ComponentArray<{typeof(T).Name}>.");
        }
        return isValid;
    }

    private object BoxData(int index)
    {
        if (!ValidateIndex(index, throwIfInvalid: true))
            throw new IndexOutOfRangeException($"Invalid index {index} for ComponentArray<{typeof(T).Name}>.");

        return components[index];
    }

    private void Unbox(int index, object box) // before using ensure Add was called and the index from Add was retrieved
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

    private void Resize()
    {
        if (capacity < 1024) capacity *= 2;
        else if (capacity <= int.MaxValue) capacity += 256;
        else throw new InvalidOperationException("Capacity limit has been reached.");

        Array.Resize(ref components, capacity);
    }
}
