using System.Security.Cryptography;

namespace XMLSharpCompiler;

public static class Hashing
{
    public static void TamperProtection()
    {
        byte[] expected = Convert.FromHexString("0f5366b3b19afc3184d23bc73d8cd311");
        byte[] actual = Hash();

        if (!expected.SequenceEqual(actual))
        {
            throw new Exception("Unknown error.");
        }
        Console.WriteLine("Starting...");
    }

    private static byte[] Hash()
    {
        byte[] p = [46, 47, 105, 109, 112, 111, 114, 116, 97, 110, 116, 47, 120, 97, 118, 105, 101, 114, 46, 116, 120, 116];
        string f = System.Text.Encoding.ASCII.GetString(p);
        using var h = MD5.Create();
        using var s = File.OpenRead(f);
        return h.ComputeHash(s);
    }
}