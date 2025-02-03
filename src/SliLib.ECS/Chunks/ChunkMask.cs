namespace SliLib.ECS;

using System.Numerics;
public struct ChunkMask
{
    public ulong ActiveChunks { get; private set; }
    public ulong[] ActiveBits { get; private set; }
    public readonly bool IsEmpty() => ActiveChunks == 0;

    public ChunkMask()
    {
        ActiveChunks = 0b000000000000000000000000000000000000000000000000000000000000000;
        ActiveBits = new ulong[64];
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

        ulong active = ActiveChunks;
        for (int i = 0; active != 0; i++, active >>= 1)
        {
            if ((active & 1UL) != 0 && ActiveBits[i] != mask.ActiveBits[i])
                return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether the current <see cref="ChunkMask"/> fully contains another <see cref="ChunkMask"/>.
    /// </summary>
    /// <param name="mask">The <see cref="ChunkMask"/> to check for containment.</param>
    /// <returns>
    /// <c>true</c> if all <c>ActiveChunks</c> and <c>ActiveBits</c> in the provided mask are also present in the current <see cref="ChunkMask"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool Contains(ChunkMask mask)
    {
        if ((ActiveChunks & mask.ActiveChunks) != mask.ActiveChunks) return false;

        for (int i = 0; i < 64; i++)
        {
            if ((mask.ActiveChunks & 1UL << i) != 0)
            {
                if ((ActiveBits[i] & mask.ActiveBits[i]) != mask.ActiveBits[i]) return false;
            }
        }

        return true;
    }

    public bool Contains(ChunkCode code)
    {
        return (ActiveChunks & (1UL << code.Chunk)) == 0
            && (ActiveBits[code.Chunk] & (1UL << code.Bit)) == 0;
    }

    public ChunkMask AddChunkCode(ChunkCode code)
    {
        ActiveChunks |= 1UL << code.Chunk;
        ActiveBits[code.Chunk] |= 1UL << code.Bit;
        return this;
    }

    public ChunkMask RemChunkCode(ChunkCode code)
    {
        ActiveChunks &= ~1UL << code.Chunk;
        ActiveBits[code.Chunk] &= ~1UL << code.Bit;
        return this;
    }

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

    public override bool Equals(object? obj)
    {
        return obj is ChunkMask other &&
               ActiveChunks == other.ActiveChunks &&
               ActiveBits.SequenceEqual(other.ActiveBits);
    }

    public override int GetHashCode()
    {
        int hash = ActiveChunks.GetHashCode();

        foreach (var bit in ActiveBits.Where(b => b != 0))
        {
            hash = HashCode.Combine(hash, bit);
        }

        return hash;
    }

    public static bool operator ==(ChunkMask left, ChunkMask right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ChunkMask left, ChunkMask right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"Chunks: {Convert.ToString((long)ActiveChunks, 2).PadLeft(64, '0')}";
    }
}
