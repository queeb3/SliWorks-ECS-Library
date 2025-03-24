using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SliLib.DataStructures;

// pure bitwise index sorting encoder
internal unsafe struct IndexEncoder
{
    private ulong* bits;
    private int length;
    private int elementSize;
    private int size;
    private int maxIndex;

    public readonly int Size => size;
    public readonly int BitsCount => length;

    [SkipLocalsInit]
    public IndexEncoder(int indexCount)
    {
        indexCount = (indexCount ^ (indexCount >> 31)) - (indexCount >> 31); // branchless absolute value
        maxIndex = indexCount;
        length = indexCount / 64;
        if (length == 0) length = 1;
        elementSize = sizeof(ulong);
        size = elementSize * length;

        bits = (ulong*)Marshal.AllocHGlobal(size);

        Unsafe.InitBlock(bits, 0, (uint)size);
    }

    public void Set(int index)
    {
        bits[index >> 6] |= 1UL << (index & 0b111111);
    }

    public void Unset(int index)
    {
        bits[index >> 6] &= ~(1UL << (index & 0b111111));
    }

    public int FirstFreeBit()
    {
        int index = -1;
        for (int i = 0; i < length; i++)
        {
            ulong inverted = ~bits[i];
            if (inverted != 0) index = (i * 64) + BitOperations.TrailingZeroCount(inverted);
        }
        return (maxIndex <= index) ? -1 : index;
    }

    public int FirstOccupiedBit()
    {
        int index = -1;
        for (int i = 0; i < length; i++)
        {
            ulong word = bits[i];
            if (word != 0) index = (i * 64) + BitOperations.TrailingZeroCount(word);
        }
        return (maxIndex <= index) ? -1 : index;
    }

    public int LastFreeBit()
    {
        for (int i = length - 1; i >= 0; i--)
        {
            ulong inverted = ~bits[i];

            if (i == length - 1 && (maxIndex % 64) != 0)
            {
                ulong mask = (1UL << (maxIndex % 64)) - 1;
                inverted &= mask;
            }

            if (inverted != 0)
            {
                return (i * 64) + (63 - BitOperations.LeadingZeroCount(inverted));
            }
        }
        return -1;
    }

    public int LastOccupiedBit()
    {
        for (int i = length - 1; i >= 0; i--)
        {
            ulong word = bits[i];

            if (i == length - 1 && (maxIndex % 64) != 0)
            {
                ulong mask = (1UL << (maxIndex % 64)) - 1;
                word &= mask;
            }

            if (word != 0)
            {
                return (i * 64) + (63 - BitOperations.LeadingZeroCount(word));
            }
        }
        return -1;
    }

    public int PopCount()
    {
        int x = 0;
        for (int i = 0; i < length; i++)
            x += BitOperations.PopCount(bits[i]);

        return x;
    }

    public int EmptyCount()
    {
        return maxIndex - PopCount();
    }

    public bool IsOccupied(int index)
    {
        int i = index >> 6;  // find correct ulong
        int bit = index & 0b111111; // find bit position
        return (bits[i] & (1UL << bit)) != 0;
    }

    public void Expand(int newIndexCount)
    {
        if (newIndexCount < maxIndex) return;

        int newLength = newIndexCount / 64;
        if (newLength == 0) newLength = 1;

        ulong* newBits = (ulong*)Marshal.AllocHGlobal(newLength * sizeof(ulong));

        Unsafe.CopyBlock(newBits, bits, (uint)(length * sizeof(ulong)));
        Unsafe.InitBlock(newBits + length, 0, (uint)((newLength - length) * sizeof(ulong)));

        Marshal.FreeHGlobal((IntPtr)bits);
        bits = newBits;
        length = newLength;
        size = sizeof(ulong) * newLength;
        maxIndex = newIndexCount;
    }

    public void Clear()
    {
        Unsafe.InitBlock(bits, 0, (uint)size);
    }

    public void Free()
    {
        if (bits is not null)
        {
            Marshal.FreeHGlobal((nint)bits);
            bits = null;
        }
    }
}


// ─── BIT MANIPULATION ───
// &  (AND)                 -> 1 if both bits are 1, else 0           (x & y)
// |  (OR)                  -> 1 if either bit is 1                   (x | y)
// ^  (XOR)                 -> 1 if bits are different                (x ^ y)
// ~  (NOT)                 -> Inverts all bits                       (~x)
// << (Left Shift)          -> Multiplies by 2^n (shift left n bits)  (x << n)
// >> (Right Shift)         -> Divides by 2^n (shift right n bits)    (x >> n)

// ─── BITWISE ARITHMETIC ───
// Multiply by 2^n          -> x << n                               Equivalent to x * 2^n
// Divide by 2^n            -> x >> n                               Equivalent to x / 2^n
// Modulo by power of 2     -> x & (2^n - 1)  // x % 2^n
// Swap without temp        -> x ^= y, y ^= x, x ^= y
// Absolute value           -> (x ^ (x >> 31)) - (x >> 31)          Works for signed ints

// ─── BIT MASKING ───
// Isolate lowest set bit   -> x & -x
// Clear lowest set bit     -> x & (x - 1)
// Set nth bit              -> x | (1 << n)
// Clear nth bit            -> x & ~(1 << n)
// Toggle nth bit           -> x ^ (1 << n)
// Check if nth bit is set  -> (x & (1 << n)) != 0

// ─── LOGICAL ROTATIONS ───
// Rotate left (ROL)        -> (x << n) | (x >> (bit_size - n))
// Rotate right (ROR)       -> (x >> n) | (x << (bit_size - n))

// ─── COMPOUND OPERATORS ───
// &= (AND Assign)          -> x = x & y
// |= (OR Assign)           -> x = x | y
// ^= (XOR Assign)          -> x = x ^ y
// <<= (LS Assign)          -> x = x << y
// >>= (RS Assign)          -> x = x >> y
// *= (Multiply)            -> x = x * y
// /= (Divide)              -> x = x / y
// %= (Modulo)              -> x = x % y

// ─── OTHER BINARY TRICKS ───
// x & (x - 1)              -> Clears lowest set bit
// x | (x + 1)              -> Sets lowest unset bit
// x ^ (x >> 1)             -> Gray code conversion
// x & -x                   -> Isolates lowest set bit
// (x & (1 << n))           -> Checks if nth bit is set
// x |= (1 << n)            -> Sets nth bit
// x &= ~(1 << n)           -> Clears nth bit
// x ^= (1 << n)            -> Toggles nth bit

// ─── MISC ───
// Fast min                 -> y ^ ((x ^ y) & -(x < y))
// Fast max                 -> x ^ ((x ^ y) & -(x < y))
// Sign retrieval           -> (x >> 31) | (x != 0)
// Bit count                -> x - ((x >> 1) & 0x55555555) - ((x >> 2) & 0x33333333) - ((x >> 3) & 0x0F0F0F0F)
