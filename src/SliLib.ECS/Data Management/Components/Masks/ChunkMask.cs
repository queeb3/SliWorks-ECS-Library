using System.Numerics;

namespace SliLib.ECS;

/// <summary>
/// Identifier for Archetypes and Systems to communicate what they have or what they want.
/// <br/><br/>
/// Systems use it to find specific components in all archetypes.<br/>
/// Archetypes use it to describe what they contain internally.
/// </summary>
public struct ChunkMask
{
    /// <summary>
    /// Represents which Chunks are active indicating a Bit is present under that Chunk index.
    /// </summary>
    public ulong ActiveChunks { get; private set; }
    /// <summary>
    /// An array of 64 ulongs with only Codes that have been added.
    /// </summary>
    public ulong[] ActiveBits { get; private set; }
    private int hashCode;

    public ChunkMask()
    {
        ActiveChunks = 0b000000000000000000000000000000000000000000000000000000000000000;
        ActiveBits = new ulong[64];
        hashCode = -1;
        GetHashCode();
    }

    /// <summary>
    /// Compares the ActiveChunks and first set of ActiveBits in both masks.
    /// <br/><br/>
    /// NOTE: This is only used for quick checks to narrow a large list.
    /// </summary>
    /// <returns><c>True</c> if chunks and first 64 bits are equal, else <c>False</c>.</returns>
    public bool SoftCompare(ChunkMask mask)
    {
        return ActiveChunks == mask.ActiveChunks
            && ActiveBits[0] == mask.ActiveBits[0];
    }

    /// <summary>
    /// Compares the current <see cref="ChunkMask"/> with another to check if they are identical.
    /// </summary>
    /// <param name="mask">The <see cref="ChunkMask"/> to compare with.</param>
    /// <returns>
    /// <c>true</c> if both <see cref="ChunkMask"/> instances have identical <c>ActiveChunks</c> and <c>ActiveBits</c>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool Matches(ChunkMask mask)
    {
        if (ActiveChunks != mask.ActiveChunks) return false;

        var span1 = ActiveBits.AsSpan();
        var span2 = mask.ActiveBits.AsSpan();

        return span1.SequenceEqual(span2);
    }

    /// <summary>
    /// Determines whether the current <see cref="ChunkMask"/> fully contains another <see cref="ChunkMask"/>.
    /// </summary>
    /// <param name="mask">The <see cref="ChunkMask"/> to check for containment.</param>
    /// <returns>
    /// <c>True</c> if all ActiveChunks and ActiveBits in the provided mask are also present in the current <see cref="ChunkMask"/>, else <c>False</c>.
    /// </returns>
    public bool Contains(ChunkMask mask)
    {
        if ((ActiveChunks & mask.ActiveChunks) != mask.ActiveChunks) return false;

        var chunksToCheck = 64 - BitOperations.LeadingZeroCount(mask.ActiveChunks);

        for (int i = 0; i < chunksToCheck; i++)
        {
            if ((mask.ActiveChunks & (1UL << i)) != 0) // Only check chunks active in mask
            {
                if ((ActiveBits[i] & mask.ActiveBits[i]) != mask.ActiveBits[i]) return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines if a <see cref="ChunkCode"/> is contained inside current <see cref="ChunkMask"/>.
    /// </summary>
    /// <returns><c>True</c> if the Chunk and Bit in <see cref="ChunkCode"/> are 1, else <c>False</c>.</returns>
    public bool Contains(ChunkCode code)
    {
        return (ActiveChunks & (1UL << code.Chunk)) != 0
            && (ActiveBits[code.Chunk] & (1UL << code.Bit)) != 0;
    }

    public bool IsEmpty() => ActiveChunks == 0;

    /// <summary>
    /// Adds a <see cref="ChunkCode"/> to the current <see cref="ChunkMask"/>.
    /// <br/><br/>
    /// NOTE: Adding the same code doesn't do anything.
    /// </summary>
    /// <returns><c>this</c>, ChunkMask for chaining.</returns>
    public ChunkMask Add(ChunkCode code)
    {
        ActiveChunks |= 1UL << code.Chunk;
        ActiveBits[code.Chunk] |= 1UL << code.Bit;

        hashCode = -1;
        GetHashCode();

        return this;
    }

    /// <summary>
    /// Removes a <see cref="ChunkCode"/> from current <see cref="ChunkMask"/>.
    /// <br/><br/>
    /// NOTE: Removing the same does nothing.
    /// </summary>
    /// <param name="code"></param>
    /// <returns><c>this</c>, ChunkMask for chaining.</returns>
    public ChunkMask Remove(ChunkCode code)
    {
        ActiveChunks &= ~(1UL << code.Chunk);
        ActiveBits[code.Chunk] &= ~(1UL << code.Bit);

        hashCode = -1;
        GetHashCode();

        return this;
    }

    /// <summary>
    /// Retreives all currently active codes inside the current <see cref="ChunkMask"/>.
    /// </summary>
    /// <returns><c>Enumerable</c> of <see cref="ChunkCode"/>s.</returns>
    public IEnumerable<ChunkCode> Codes()
    {
        for (int i = 0; i < ActiveBits.Length; i++)
        {
            if ((ActiveChunks & (1UL << i)) == 0) continue;

            ulong bits = ActiveBits[i];
            while (bits != 0)
            {
                int b = BitOperations.TrailingZeroCount(bits);
                yield return new ChunkCode(i, b);
                bits &= ~(1UL << b);
            }
        }
    }

    // ----------------------------------------Hash & Equals---------------------------------------------

    public override bool Equals(object? obj) => obj is ChunkMask other &&
                                                ActiveChunks == other.ActiveChunks &&
                                                ActiveBits.SequenceEqual(other.ActiveBits);

    public override int GetHashCode()
    {
        if (hashCode != -1) return hashCode;

        hashCode = ActiveBits.Aggregate(ActiveChunks.GetHashCode(), HashCode.Combine);
        return hashCode;
    }

    public static bool operator ==(ChunkMask left, ChunkMask right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ChunkMask left, ChunkMask right)
    {
        return !(left == right);
    }

    // ----------------------------------------Utility---------------------------------------------

    public override string ToString()
    {
        return $"Chunks: {Convert.ToString((long)ActiveChunks & 0x7FFFFFFFFFFFFFFF, 2).PadLeft(64, '0')}";
    }

    /// <summary>
    /// A debug method to visually see in console what Chunks and Bits are currently flipped to 1 based on a Chunk index; 0-63.
    /// </summary>
    /// <returns><c>string</c> representation of bits as a binary.</returns>
    public string PrintChunkBits(int chunkIndex)
    {
        if (chunkIndex < 0 || chunkIndex >= ActiveBits.Length)
        {
            return $"Chunk {chunkIndex}: [Invalid Index]";
        }

        if ((ActiveChunks & (1UL << chunkIndex)) == 0)
        {
            return $"Chunk {chunkIndex}: [Inactive]";
        }

        string bitString = Convert.ToString((long)ActiveBits[chunkIndex], 2).PadLeft(64, '0');
        return $"Chunk {chunkIndex}: {bitString}";
    }
}
