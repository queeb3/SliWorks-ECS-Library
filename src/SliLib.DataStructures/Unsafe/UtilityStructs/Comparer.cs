namespace SliLib.DataStructures;

internal unsafe struct Externals
{
    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int memcmp(void* ptr1, void* ptr2, UIntPtr count); // byte comparison since == doesnt work

    public static bool Compare<T>(T* item1, T* item2) where T : unmanaged
    {
        byte* itemPtr = (byte*)&item1; // pointer to address
        byte* itemPtr = (byte*)&item2;

        if (memcmp(item1, item2, (UIntPtr)sizeof(T)) == 0) return true;

        return false;
    }
}
