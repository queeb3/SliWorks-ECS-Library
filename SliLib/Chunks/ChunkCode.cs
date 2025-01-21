namespace SliLib.Chunks;

public readonly struct ChunkCode(int chunk, int bit)
{
    public int Chunk { get; } = chunk; // The index of the chunk
    public int Bit { get; } = bit;  // The position of the bit within the chunk

    public override string ToString()
    {
        return $"{Chunk}|{Bit}";
    }

    public override bool Equals(object? obj)
    {
        return obj is ChunkCode other && Chunk == other.Chunk && Bit == other.Bit;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Chunk, Bit);
    }

    public static bool operator ==(ChunkCode left, ChunkCode right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ChunkCode left, ChunkCode right)
    {
        return !(left == right);
    }
}
