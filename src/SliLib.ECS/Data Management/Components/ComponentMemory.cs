using System.Runtime.CompilerServices;

namespace SliLib.ECS;

public ref struct ComponentMemory<T> where T : struct
{
    private Span<T> components;
    public int Length { get => components.Length; }

    public ComponentMemory(ref Array compArr)
    {
        components = Unsafe.As<T[]>(compArr).AsSpan();
    }

    public ref T this[int index]
    {
        get => ref GetRef(index);
    }

    private ref T GetRef(int index)
    {
        return ref components[index];
    }

    public Span<T> GetSlice(int low, int high)
    {
        return components.Slice(low, high);
    }
}
