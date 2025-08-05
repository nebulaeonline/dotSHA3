# dotSHA3

dotSHA3 is a wrapper around Keccak's optimized SHA-3 C implementation as contained in libXKCP.

It provides both one-shot and incremental hashing (for streaming data).

Span-based, cross-platform, and verified against the NIST test vectors for each hash length (224/256/384/512) 100% (860/860) passing on all platforms.

In benchmarking, this library is exactly the same speed as Microsoft's System.Security.Cryptography SHA3 implementation, but supports stream hashing, which the built-in hasher does not.

Tests and benchmarks are included in the Gihub repository.

[![NuGet](https://img.shields.io/nuget/v/nebulae.dotSHA3.svg)](https://www.nuget.org/packages/nebulae.dotSHA3)

## Features

- **Cross-platform**: Works on Windows, Linux, and macOS (x64 & Apple Silicon).
- **High performance**: Optimized for speed, leveraging native SIMD-enabled code.
- **Easy to use**: Simple API for generating keys and signing/verification.
- **Secure**: Uses Keccak XKCP implementation, which is widely trusted in the industry.
- **Minimal dependencies**: No external dependencies required (all are included), making it lightweight and easy to integrate.

---

## Requirements

- .NET 8.0 or later
- Windows x64, Linux x64, or macOS (x64 & Apple Silicon)

---

## Usage

Non-streaming (one-shot) hashing:

```csharp

using System;
using System.Text;
using nebulae.dotSHA3;

class Program
{
    static void Main()
    {
        var input = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog");

        using var sha3 = new SHA3(SHA3Algorithm.Sha3_256);
        var hash = sha3.ComputeHash(input);

        Console.WriteLine("SHA3-256:");
        Console.WriteLine(Convert.ToHexString(hash));
    }
}

```

Stream hashing:

```csharp

using System;
using System.Text;
using nebulae.dotSHA3;

class Program
{
    static void Main()
    {
        var chunk1 = Encoding.UTF8.GetBytes("The quick brown ");
        var chunk2 = Encoding.UTF8.GetBytes("fox jumps over ");
        var chunk3 = Encoding.UTF8.GetBytes("the lazy dog");

        using var sha3 = new SHA3(SHA3Algorithm.Sha3_256);
        sha3.Update(chunk1);
        sha3.Update(chunk2);
        sha3.Update(chunk3);
        var hash = sha3.FinalizeHash();

        Console.WriteLine("SHA3-256 (streamed):");
        Console.WriteLine(Convert.ToHexString(hash));
    }
}

```

```csharp

using System;
using System.IO;
using nebulae.dotSHA3;

class Program
{
    static void Main(string[] args)
    {
        const string path = "largefile.bin";
        Span<byte> buffer = stackalloc byte[8192];

        using var sha3 = new SHA3(SHA3Algorithm.Sha3_512);
        using var fs = File.OpenRead(path);

        int bytesRead;
        while ((bytesRead = fs.Read(buffer)) > 0)
        {
            sha3.Update(buffer[..bytesRead]);
        }

        var hash = sha3.FinalizeHash();
        Console.WriteLine($"SHA3-512: {Convert.ToHexString(hash)}");
    }
}

```

---

## Installation

You can install the package via NuGet:

```bash

$ dotnet add package nebulae.dotSHA3

```

Or via git:

```bash

$ git clone https://github.com/nebulaeonline/dotSHA3.git
$ cd dotSHA3
$ dotnet build

```

---

## License

MIT

## Roadmap

Unless there are vulnerabilities found, there are no plans to add any new features.