namespace SliLib.ECS;

public class Chunk
{
    public ulong Chunks { get; private set; } = 0b0000000000000000000000000000000000000000000000000000000000000000;
    public ulong[] ChunkBits { get; private set; } = new ulong[64];

    public ChunkCode Register<T>(uint id) where T : struct
    {
        if (id == 0) throw new InvalidDataException($"Id must be greater than 0.");

        var code = IdToCode(id);

        SetChunk(code.Chunk);
        SetBit(code.Chunk, code.Bit);

        return code;
    }

    public ChunkCode GetCode(uint id)
    {
        var code = IdToCode(id);

        if (!IdActive(id))
        {
            throw new InvalidOperationException($"The code for id {id} is not assigned to any component.");
        }

        return code;
    }

    public bool IdActive(uint id)
    {
        var code = IdToCode(id);

        if ((Chunks & (1UL << code.Chunk)) == 0)
            return false;

        if ((ChunkBits[code.Chunk] & (1UL << code.Bit)) == 0)
            return false;

        return true;
    }

    private void SetChunk(int chunkShifts)
    {
        Chunks |= 1UL << chunkShifts;
    }

    private void SetBit(int chunk, int bitShifts)
    {
        if ((ChunkBits[chunk] & 1UL << bitShifts) != 0)
        {
            throw new IndexOutOfRangeException(
                $"Cannot set a bit over an already existing bit.\n" +
                $"Chunk: {chunk} | Bit Position: {bitShifts}\n" +
                $"Current Chunk Value: {Convert.ToString((long)ChunkBits[chunk], 2).PadLeft(64, '0')}");
        }

        ChunkBits[chunk] |= 1UL << bitShifts;
    }

    private static ChunkCode IdToCode(uint id)
    {
        int chunkIndex = (int)(id / 64); // Determine the chunk index
        if (chunkIndex > 63) throw new IndexOutOfRangeException("Chunk size cannot exceed 64 total chunks.");

        int bitIndex = (int)(id % 64); // Determine the bit position within the chunk
        return new(chunkIndex, bitIndex);
    }
}
