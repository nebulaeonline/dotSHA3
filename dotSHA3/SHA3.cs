using nebulae.dotSHA3;

public sealed class SHA3 : IDisposable
{
    private Sha3_1x_Ctx _ctx;
    private readonly int _digestLength;
    private bool _finalized;
    private readonly SHA3Algorithm _variant;

    static SHA3()
    {
        SHA3Library.Init();
    }

    public SHA3(SHA3Algorithm variant)
    {
        _variant = variant;
        _ctx = default;

        _digestLength = variant switch
        {
            SHA3Algorithm.Sha3_224 => 28,
            SHA3Algorithm.Sha3_256 => 32,
            SHA3Algorithm.Sha3_384 => 48,
            SHA3Algorithm.Sha3_512 => 64,
            _ => throw new ArgumentOutOfRangeException(nameof(variant))
        };

        SHA3Interop.sha3_1x_init(ref _ctx, _variant);
    }

    public void Update(ReadOnlySpan<byte> data)
    {
        EnsureNotFinalized();
        byte[] temp = data.ToArray(); // interop requires byte[]
        SHA3Interop.sha3_1x_update(ref _ctx, temp, (UIntPtr)temp.Length);
    }

    public byte[] FinalizeHash()
    {
        EnsureNotFinalized();
        var output = new byte[_digestLength];
        SHA3Interop.sha3_1x_final(ref _ctx, output);
        _finalized = true;
        return output;
    }

    public byte[] ComputeHash(ReadOnlySpan<byte> data)
    {
        Reset();
        Update(data);
        return FinalizeHash();
    }

    private void Reset()
    {
        _finalized = false;
        SHA3Interop.sha3_1x_init(ref _ctx, _variant);
    }

    public void Dispose() => _finalized = true;
    private void EnsureNotFinalized()
    {
        if (_finalized)
            throw new InvalidOperationException("Hash already finalized");
    }
}
