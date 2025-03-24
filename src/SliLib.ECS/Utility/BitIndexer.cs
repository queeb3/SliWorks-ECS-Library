using System.Numerics;
using System.Runtime.CompilerServices;

namespace SliLib.ECS;

/// <summary>
/// Tracks set and unset indexes to find free slots to place new data.
/// <br/><br/>
/// USES:<br/>
/// - Free or Full indexes such as an array of arrays with set sizes<br/>
/// - Finding gaps in an array for compacting data<br/>
/// - and more that you can figure out :D
/// </summary>
public class BitIndexer
{
    private ulong[] Bits; // ulongs are more compact and can be checked in loops faster

    /// <summary>
    /// The bounds of the external array. ie: Capacity.<br/>
    /// Divided by 64 to get actual length if the bit array.
    /// </summary>
    public int TrackingCount { get; private set; }
    public int HighestBitAllocated { get; private set; }

    // this cap should be set to the cap of the array its meant to track divided by 64
    public BitIndexer(int tracking)
    {
        Bits = new ulong[(tracking >> 6) + 63]; // buffer offset
        TrackingCount = tracking;
    }

    /// <summary>
    /// Checks if a index is full or set.
    /// </summary>
    /// <returns><c>True</c> if bit index is set.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOccupied(int index)
    {
        var b = IndexToBit(index, out var a);
        return (Bits[a] & (1UL << b)) >> b == 1;
    }

    /// <summary>
    /// Finds the first bit in sequence that is unset.
    /// <br/><br/>
    /// NOTE: this can be a full Array or an item is already at this index. - Occupied|Full
    /// </summary>
    /// <returns>An <c>int</c> representation of which bit is free or -1 for notfound/outofbounds</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int FindFreeIndex()
    {
        int index;
        for (int i = 0; i < Bits.Length; i++)
        {
            var bit = ~Bits[i]; // invert
            if (bit == 0) continue; // skip full

            index = (i << 6) + BitOperations.TrailingZeroCount(bit);

            if (index >= TrackingCount) return -1; // out of bounds
            return index;
        }

        return -1; // not free bit found;

        // -1 can be used to call a resize or something like a fix in your own implementations
    }

    /// <summary>
    /// Sets a bit to 0, marking it as available.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unset(int index)
    {
        Bits[index >> 6] &= ~(1UL << (index & 0b111111));
    }

    /// <summary>
    /// Sets a bit to 1, marking it as Occupied.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int index)
    {
        Bits[index >> 6] |= 1UL << (index & 0b111111);
    }

    /// <summary>
    /// Converts and int index to a bit and array index.
    /// <br/><br/>
    /// NOTE: Has no attachment to the bit array in this instance.
    /// </summary>
    /// <returns>An <c>int</c> for bit; <c>Out</c> is <c>array index</c> for which ulong the bit resides.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexToBit(int index, out int arrayIndex)
    {
        arrayIndex = index >> 6;
        return index & 0b111111; // bit
    }

    /// <summary>
    /// increases the size based on the new capacity of the external array.
    /// <br/><br/>
    /// DANGER: Do not set lower ot you may lose data.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Expand(int newTrack)
    {
        Array.Resize(ref Bits, (newTrack + 63) >> 6); // account for possible missing size with 63 offset
        TrackingCount = newTrack;
    }
}
