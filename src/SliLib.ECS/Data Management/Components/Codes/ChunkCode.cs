namespace SliLib.ECS;

/// <summary>
/// The Chunk and Bit location in a CodeRegister for registered components.
/// </summary>
/// <param name="chunk"></param>
/// <param name="bit"></param>
public readonly struct ChunkCode(int chunk, int bit)
{
    /// <summary>
    /// The index that points to a ulong inside an array of 64 ulongs.
    /// </summary>
    public int Chunk { get; } = chunk;
    /// <summary>
    /// The actual Bit index inside the ulong at a specific Chunk.
    /// </summary>
    public int Bit { get; } = bit;

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
