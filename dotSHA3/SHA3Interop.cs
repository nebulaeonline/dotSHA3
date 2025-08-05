using System;
using System.Runtime.InteropServices;

namespace nebulae.dotSHA3;

public enum SHA3Algorithm : int
{
    Sha3_224 = 224,
    Sha3_256 = 256,
    Sha3_384 = 384,
    Sha3_512 = 512
}

[StructLayout(LayoutKind.Sequential, Size = 392)] // adjust if your 1x ctx struct layout changes
internal unsafe struct Sha3_1x_Ctx { }

[StructLayout(LayoutKind.Sequential, Size = 1792)] // 25x32 (AVX2 state) + 4*144 buffer + metadata
internal unsafe struct Sha3_4x_Ctx { }

internal static class SHA3Interop
{
    private const string LIBRARY = "sha3"; // sha3.dll or libsha3.so/dylib

    [DllImport(LIBRARY, EntryPoint = "sha3_1x_init", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sha3_1x_init(ref Sha3_1x_Ctx ctx, SHA3Algorithm variant);

    [DllImport(LIBRARY, EntryPoint = "sha3_1x_update", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sha3_1x_update(ref Sha3_1x_Ctx ctx, byte[] data, UIntPtr len);

    [DllImport(LIBRARY, EntryPoint = "sha3_1x_final", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sha3_1x_final(ref Sha3_1x_Ctx ctx, byte[] output);


    [DllImport(LIBRARY, EntryPoint = "sha3_4x_init", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sha3_4x_init(ref Sha3_4x_Ctx ctx, SHA3Algorithm variant);

    [DllImport(LIBRARY, EntryPoint = "sha3_4x_update", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sha3_4x_update(ref Sha3_4x_Ctx ctx, IntPtr dataPtrArray, UIntPtr len);

    [DllImport(LIBRARY, EntryPoint = "sha3_4x_final", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sha3_4x_final(ref Sha3_4x_Ctx ctx, IntPtr outputPtrArray);
}
