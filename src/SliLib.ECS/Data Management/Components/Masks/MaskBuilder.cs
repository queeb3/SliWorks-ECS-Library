namespace SliLib.ECS;

/// <summary>
/// Allows easy creation of ChunkMasks through a ComponentRegister.
/// </summary>
public class ChunkMaskBuilder
{
    private ComponentRegister Reg;
    private ChunkMask mask = new();

    public ChunkMaskBuilder(ComponentRegister reg)
    {
        Reg = reg;
    }

    /// <summary>
    /// Easy chain adding for components in a register.
    /// </summary>
    public ChunkMaskBuilder Add<T>() where T : struct
    {
        mask = mask.Add(Reg.GetInfo<T>().Code);
        return this;
    }

    /// <summary>
    /// Finishes the build and resets the internal buffers.
    /// </summary>
    /// <returns>ChunkMask with added components.</returns>
    public ChunkMask Return()
    {
        if (!Reg.ValidMask(mask)) throw new InvalidOperationException($"The ComponentRegister does not hold registers for all the components added! \n Please ensure all components are registed.");

        var nMask = mask;
        mask = new();
        return nMask;
    }
}
