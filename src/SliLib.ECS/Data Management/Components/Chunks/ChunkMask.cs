using System.Numerics;

namespace SliLib.ECS;

public struct ChunkMask
{
    private static readonly int pad = Vector<ulong>.Count;

    public ulong ActiveChunks { get; private set; }
    public ulong[] ActiveBits { get; private set; }
    private int hashCode;

    public ChunkMask()
    {
        ActiveChunks = 0b000000000000000000000000000000000000000000000000000000000000000;
        ActiveBits = new ulong[64 + pad];
        hashCode = -1;
        GetHashCode();
    }

    /// <summary>
    /// Compares the current <see cref="ChunkMask"/> with another to check if they are identical.
    /// </summary>
    /// <param name="mask">The <see cref="ChunkMask"/> to compare with.</param>
    /// <returns>
    /// <c>true</c> if both <see cref="ChunkMask"/> instances have identical <c>ActiveChunks</c> and <c>ActiveBits</c>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public readonly bool Matches(ChunkMask mask)
    {
        if (ActiveChunks != mask.ActiveChunks) return false;

        int vectorSize = Vector<ulong>.Count;
        int length = ActiveBits.Length;

        // vectorized comparison loop
        for (int i = 0; i < length; i += vectorSize)
        {
            var vecA = new Vector<ulong>(ActiveBits, i);
            var vecB = new Vector<ulong>(mask.ActiveBits, i);

            if (!Vector.EqualsAll(vecA, vecB)) return false;
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
    public readonly bool Contains(ChunkMask mask)
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

    public readonly bool Contains(ChunkCode code)
    {
        return (ActiveChunks & (1UL << code.Chunk)) != 0
            && (ActiveBits[code.Chunk] & (1UL << code.Bit)) != 0;
    }

    public readonly bool IsEmpty() => ActiveChunks == 0;

    public ChunkMask Add(ChunkCode code)
    {
        ActiveChunks |= 1UL << code.Chunk;
        ActiveBits[code.Chunk] |= 1UL << code.Bit;

        hashCode = -1;
        GetHashCode();

        return this;
    }

    public ChunkMask Remove(ChunkCode code)
    {
        ActiveChunks &= ~(1UL << code.Chunk);
        ActiveBits[code.Chunk] &= ~(1UL << code.Bit);

        hashCode = -1;
        GetHashCode();

        return this;
    }

    public readonly IEnumerable<ChunkCode> Codes()
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

    public override readonly bool Equals(object? obj) => obj is ChunkMask other &&
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

    public override readonly string ToString()
    {
        return $"Chunks: {Convert.ToString((long)ActiveChunks, 2).PadLeft(64, '0')}";
    }
}
