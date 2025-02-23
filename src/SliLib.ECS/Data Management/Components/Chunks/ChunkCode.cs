namespace SliLib.ECS;

public readonly struct ChunkCode(int chunk, int bit)
{
    public int Chunk { get; } = chunk; // The index of the chunk
    public int Bit { get; } = bit;  // The position of the bit within the chunk

    public override string ToString()
    {
        return $"{Chunk}|{Bit}";
    }

    public bool Equals(ChunkCode other) => Chunk == other.Chunk && Bit == other.Bit;
    public override bool Equals(object? obj) => obj is ChunkCode other && Equals(other);

    public override int GetHashCode()
    {
        return (Chunk << 6) | Bit;
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
