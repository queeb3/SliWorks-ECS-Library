using System.Numerics;
using System.Runtime.CompilerServices;

namespace SliLib.ECS;


// for use in entity register only as there is no internal bounds checking and
// must be handled through external math
internal struct EntityBlock
{
    private readonly ulong[] ulongs;
    // 8 ulongs fit in one cache line
    // total of 512 indexes per block

    public EntityBlock()
    {
        ulongs = new ulong[8];
    }


    public bool CheckIndex(int index)
    {
        var bitState = BitLocator(index);

        return bitState == 1;
    }

    public void Set(int index)
    {
        ulongs[index >> 6] |= 1UL << (index & 0b111111);
    }

    public void Unset(int index)
    {
        ulongs[index >> 6] &= ~(1UL << (index & 0b111111));
    }

    private ulong BitLocator(int index) // returns the bit state of a ulong bit, 0 or 1
    {
        // the check for valid indexes happens above
        var bit = index & 0b111111;
        var byteIndex = index >> 6;

        return (ulongs[byteIndex] >> bit) & 1UL;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int FindFirstUnsetBitIndex()
    {
        for (int i = 0; i < 8; i++)
        {
            ulong inverted = ~ulongs[i];
            if (inverted != 0) return (i * 64) + BitOperations.TrailingZeroCount(inverted);
        }
        return -1;
    }
}

public enum EntityIdState
{
    /// <summary>
    /// Entity is active and bit is 1
    /// </summary>
    Set,
    /// <summary>
    /// Entity is not active and bit is 0; but is still allocated for use
    /// </summary>
    Unset,
    /// <summary>
    /// There is no bit at this index available and is out of range
    /// </summary>
    Unallocated
}

/// <summary>
/// Manages entity allocation and tracking using a compact bitwise storage system.
/// </summary>
public class EntityRegister
{
    private const int blockOffset = 512;

    private bool[] fullBlocks;
    private EntityBlock[] blocks;
    private readonly Dictionary<int, int> missingBits;

    private int firstUnfilledBlock;

    public int Capacity { get; private set; }
    public int Count { get; private set; } // entities created
    public int BCount { get; private set; } // block count
    public bool Resizable { get; private set; }

    // NOTE: cap is always multiplied by 512
    // NOTE: can do a math operation like 100000/512 to get actual cap

    /// <summary>
    /// Manages entity allocation and tracking using a compact bitwise storage system.
    /// Each `EntityBlock` represents 512 entities stored efficiently using bit flags. <br/>
    /// The register dynamically expands based on configuration.
    /// </summary>
    /// <param name="capacity">Initial number of entity blocks (each block holds 512 entities).</param>
    /// <param name="resizable">If `true`, the register will grow dynamically when full.</param>
    public EntityRegister(int capacity = 64, bool resizable = false)
    {
        blocks = new EntityBlock[capacity];
        fullBlocks = new bool[capacity];
        missingBits = new(32);
        Capacity = capacity;
        Resizable = resizable;

        Count = 0;
        BCount = 0;
        firstUnfilledBlock = 0;
    }

    /// <summary>
    /// Creates a new entity and assigns it a unique ID within a bit-packed `EntityBlock`. <br/>
    /// The ID is guaranteed to be unique and mapped to a bit index in one of the allocated blocks.
    /// </summary>
    /// <returns>
    /// The unique entity ID (`int`) assigned to the new entity.
    /// </returns>
    public int Create()
    {
        ref var block = ref FirstUnfilledBlock(out var bIndex);

        int bitIndex = block.FindFirstUnsetBitIndex();
        int index = (bIndex * blockOffset) + bitIndex; // start of block indexing
        int blockId = index % blockOffset;

        if (bitIndex == blockOffset - 1) fullBlocks[bIndex] = true;
        block.Set(blockId);
        Count++;
        return index;
    }

    /// <summary>
    /// Marks an entity as inactive by unsetting its bit in the register. <br/>
    /// This does not remove the entity from memory—only marks it as available for reuse.
    /// </summary>
    /// <param name="id">The entity ID to mark as inactive.</param>
    public void Destroy(int id)
    {
        if (!Valid(id)) return;

        int blockIndex = id / blockOffset;
        if (blockIndex == -1) return;

        ref var block = ref blocks[blockIndex];
        int convertedId = id % blockOffset;

        fullBlocks[blockIndex] = false;
        block.Unset(convertedId);
        Count--;

        if (!missingBits.TryGetValue(blockIndex, out var ctr))
        {
            missingBits[blockIndex] = 1;
        }
        missingBits[blockIndex] = ++ctr;

        if (blockIndex < firstUnfilledBlock) firstUnfilledBlock = blockIndex;
    }

    /// <summary>
    /// Retrieves metadata for an existing entity, if it is currently active. <br/>
    /// If the entity exists, an `EntityInfo` struct is populated with its details.
    /// </summary>
    /// <param name="id">The entity ID to query.</param>
    /// <param name="info">Outputs an `EntityInfo` struct containing the entity's metadata.</param>
    /// <returns>
    /// `true` if the entity is active and valid; otherwise, `false`.
    /// </returns>
    public bool GenerateInfoFromId(int id, out EntityInfo info)
    {
        if (CheckIdState(id) == EntityIdState.Set)
        {
            info = new(id);
            return true;
        }

        info = new(-1);
        return false;
    }

    /// <summary>
    /// Determines the current state of an entity within the register. <br/>
    /// Entities can be: <br/>
    /// - `Set` (active) <br/>
    /// - `Unset` (inactive but still allocated) <br/>
    /// - `Unallocated` (ID is out of range or never assigned)
    /// </summary>
    /// <param name="id">The entity ID to check.</param>
    /// <returns>An `EntityIdState` representing the entity's current status.</returns>
    public EntityIdState CheckIdState(int id)
    {
        if (!Valid(id)) return EntityIdState.Unallocated;

        if (!CheckIdBit(id)) return EntityIdState.Unset;

        return EntityIdState.Set;
    }

    // ------------------------------Utility Methods--------------------------------------

    private ref EntityBlock FirstUnfilledBlock(out int bIndex)
    {
        if (BCount == 0) return ref CreateNewBlock(out bIndex); // first block creation


        if (missingBits.TryGetValue(firstUnfilledBlock, out var ctr) && !fullBlocks[firstUnfilledBlock])
        {
            if (--ctr > 0)
            {
                missingBits[firstUnfilledBlock] = ctr;
                bIndex = firstUnfilledBlock;
                return ref blocks[bIndex];
            }
            else
            {
                missingBits.Remove(firstUnfilledBlock);
                fullBlocks[firstUnfilledBlock] = true;
            }
        }

        // fall back search for next block thats not full from last unfilled
        for (int i = firstUnfilledBlock; i < BCount; i++)
        {
            if (!fullBlocks[i])
            {
                firstUnfilledBlock = i;
                bIndex = i;
                return ref blocks[i];
            }
        }

        return ref CreateNewBlock(out bIndex);
    }

    private ref EntityBlock CreateNewBlock(out int newBIndex)
    {
        if (!Resizable && BCount == Capacity) throw new EntityLimitReachedException();
        else if (BCount == Capacity) Resize();

        blocks[BCount++] = new();
        newBIndex = BCount - 1;
        return ref blocks[newBIndex];
    }

    private bool CheckIdBit(int id)
    {
        var blockIndex = Valid(id) ? id / blockOffset : -1;
        if (blockIndex == -1) return false;

        ref var block = ref blocks[blockIndex];
        var convertedId = id % blockOffset;

        return block.CheckIndex(convertedId);
    }

    private bool Valid(int id)
    {
        if (id >= 0 && id < BCount * blockOffset) return true;
        return false;
    }

    private void Resize()
    {
        int newSize = Capacity * 2;
        var newBlocks = GC.AllocateUninitializedArray<EntityBlock>(newSize);
        var newFull = GC.AllocateUninitializedArray<bool>(newSize);
        Array.Copy(blocks, newBlocks, blocks.Length);
        Array.Copy(fullBlocks, newFull, fullBlocks.Length);
        blocks = newBlocks;
        fullBlocks = newFull;
        Capacity = newSize;
    }
}
