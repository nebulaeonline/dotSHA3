using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotSHA3
{
    internal static class SHA3Library
    {
        private static bool _isLoaded;

        internal static void Init()
        {
            if (_isLoaded)
                return;

            var libName = GetPlatformLibraryName();
            var assemblyDir = Path.GetDirectoryName(typeof(SHA3Library).Assembly.Location)!;
            var fullPath = Path.Combine(assemblyDir, libName);

            if (!File.Exists(fullPath))
                throw new DllNotFoundException($"Could not find native SHA3 library at {fullPath}");

            NativeLibrary.Load(fullPath);
            _isLoaded = true;
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
