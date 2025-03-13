namespace SliLib.ECS;

internal class CodeRegister
{
    public ulong Chunks { get; private set; } = 0b0000000000000000000000000000000000000000000000000000000000000000;
    public ulong[] ChunkBits { get; private set; } = new ulong[64];
    public const int MaxComponents = 4096;
    public ChunkMask BaseMask { get; private set; } = new(); // allows to see if a mask is allowed from this register.

    public ChunkCode Register(int id)
    {
        var code = IdToCode(id);

        SetChunk(code.Chunk);
        SetBit(code.Chunk, code.Bit);

        BaseMask = BaseMask.Add(code);

        return code;
    }

    public bool IdActive(int id)
    {
        var code = IdToCode(id);
        return (Chunks & (1UL << code.Chunk)) != 0 &&
               (ChunkBits[code.Chunk] & (1UL << code.Bit)) != 0;
    }

    private void SetChunk(int chunkShifts)
    {
        Chunks |= 1UL << chunkShifts;
    }

    private void SetBit(int chunk, int bitShifts)
    {
        ulong bitMask = 1UL << bitShifts;
        if ((ChunkBits[chunk] & bitMask) != 0) return;

        ChunkBits[chunk] |= bitMask;
    }

    public static ChunkCode IdToCode(int id)
    {
        if (id < 0 || id > MaxComponents) throw new InvalidOperationException($"Component Id {id} is out of range.");
        int chunkIndex = id >> 6; // Determine the chunk index
        int bitIndex = id & 0b111111; // Determine the bit position within the chunk
        return new(chunkIndex, bitIndex);
    }
    public static int CodeToId(ChunkCode code) => code.Bit | (code.Chunk << 6);
}
