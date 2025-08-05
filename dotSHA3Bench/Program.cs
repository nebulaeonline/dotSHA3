using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using nebulae.dotSHA3;
using System.Security.Cryptography;

[MemoryDiagnoser]
public class Sha3Benchmarks
{
    private byte[] input;

    [GlobalSetup]
    public void Setup()
    {
        input = new byte[1024];
        Random.Shared.NextBytes(input);
    }

    [Benchmark(Baseline = true)]
    public byte[] DotNet_SHA3_256()
    {
        using var sha3 = SHA3_256.Create();
        return sha3.ComputeHash(input);
    }

    [Benchmark]
    public byte[] DotSHA3_AVX2()
    {
        using var sha3 = new SHA3(SHA3Algorithm.Sha3_256);
        return sha3.ComputeHash(input);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<Sha3Benchmarks>();
    }
}
