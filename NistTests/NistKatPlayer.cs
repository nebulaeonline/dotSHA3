using System.Globalization;

using nebulae.dotSHA3;

namespace NistTests;

internal class NistKatPlayer
{
    public static void RunSha3Kat(string path)
    {
        using var reader = new StreamReader(path);
        string? line;
        SHA3Algorithm? currentAlgorithm = null;

        int testCount = 0;
        int failCount = 0;
        int bitLength = -1;

        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();

            if (line.StartsWith("Len ="))
            {
                bitLength = int.Parse(line.Substring(5).Trim());
            }

            if (line.StartsWith("[L ="))
            {
                if (line.Contains("224")) currentAlgorithm = SHA3Algorithm.Sha3_224;
                else if (line.Contains("256")) currentAlgorithm = SHA3Algorithm.Sha3_256;
                else if (line.Contains("384")) currentAlgorithm = SHA3Algorithm.Sha3_384;
                else if (line.Contains("512")) currentAlgorithm = SHA3Algorithm.Sha3_512;
                else throw new Exception($"Unknown digest size in line: {line}");
            }

            if (line.StartsWith("Msg ="))
            {
                if (bitLength < 0)
                    throw new Exception("No valid Len = line before Msg");

                var msgHex = line.Substring(5).Trim();
                byte[] msg = HexToBytes(msgHex);

                // Truncate to bitLength
                int byteLen = (bitLength + 7) / 8;
                if (msg.Length > byteLen)
                    Array.Resize(ref msg, byteLen);

                int bitsExtra = bitLength % 8;
                if (bitsExtra > 0 && msg.Length > 0)
                {
                    int mask = 0xFF << (8 - bitsExtra);
                    msg[^1] &= (byte)mask;
                }

                string? mdLine;
                do { mdLine = reader.ReadLine(); } while (mdLine != null && !mdLine.TrimStart().StartsWith("MD ="));
                if (mdLine == null) throw new Exception("Expected MD = line");

                var expectedHex = mdLine.Substring(5).Trim();
                byte[] expected = HexToBytes(expectedHex);

                if (currentAlgorithm == null) throw new Exception("Digest length (L =) not declared before test block");

                using var sha3 = new SHA3(currentAlgorithm.Value);
                byte[] actual = sha3.ComputeHash(msg);

                testCount++;
                if (!ByteArraysEqual(actual, expected))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"FAIL ({currentAlgorithm}) - Test #{testCount}");
                    Console.WriteLine($"Len = {bitLength}");
                    Console.WriteLine($"Input = {BitConverter.ToString(msg)}");
                    Console.WriteLine($"Expected = {BitConverter.ToString(expected)}");
                    Console.WriteLine($"Actual   = {BitConverter.ToString(actual)}");
                    Console.ResetColor();
                    failCount++;
                }

                // reset for next round
                bitLength = -1;
            }
        }

        Console.WriteLine($"Completed {testCount} test vectors ({failCount} failed)");
    }

    private static bool ByteArraysEqual(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    {
        return a.Length == b.Length && a.SequenceEqual(b);
    }

    private static byte[] HexToBytes(string hex)
    {
        if (hex.Length % 2 != 0)
            hex = hex + "0"; // pad if needed (shouldn't happen in KATs)

        byte[] result = new byte[hex.Length / 2];
        for (int i = 0; i < result.Length; i++)
            result[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
        return result;
    }
}
