using System.Reflection;
using System.Runtime.InteropServices;

namespace nebulae.dotSHA3
{
    internal static class SHA3Library
    {
        private static bool _isLoaded;

        internal static void Init()
        {
            if (_isLoaded)
                return;

            NativeLibrary.SetDllImportResolver(typeof(SHA3Library).Assembly, Resolve);

            _isLoaded = true;
        }

        private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName != "sha3")
                return IntPtr.Zero;

            var libName = GetPlatformLibraryName();
            var assemblyDir = Path.GetDirectoryName(typeof(SHA3Library).Assembly.Location)!;
            var fullPath = Path.Combine(assemblyDir, libName);

            if (!File.Exists(fullPath))
                throw new DllNotFoundException($"Could not find native SHA3 library at {fullPath}");

            return NativeLibrary.Load(fullPath);
        }

        private static string GetPlatformLibraryName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Path.Combine("runtimes", "win-x64", "native", "sha3.dll");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return Path.Combine("runtimes", "linux-x64", "native", "libsha3.so");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                    return Path.Combine("runtimes", "osx-arm64", "native", "libsha3.dylib");

                return Path.Combine("runtimes", "osx-x64", "native", "libsha3.dylib");
            }

            throw new PlatformNotSupportedException("Unsupported platform");
        }
    }
}
