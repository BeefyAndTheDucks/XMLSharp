using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace XMLSharpCompiler;

public static class Hashing
{
    public static void TamperProtection()
    {
        try
        {
            byte[] expected = Convert.FromHexString("a95dc4e899ac156e6e0b6666dd970c9959e0e4fd948177c263543b23492768ec");
            byte[] actual = Hash();

            if (!expected.SequenceEqual(actual))
            {
                Fail();
            }
        }
        catch
        {
            Fail();
        }
    }

    private static byte[] Hash()
    {
        byte[] p = [46, 47, 105, 109, 112, 111, 114, 116, 97, 110, 116, 47, 120, 97, 118, 105, 101, 114, 46, 116, 120, 116];
        string f = Encoding.ASCII.GetString(p);

        using var h = SHA256.Create();
        using var s = File.OpenRead(f);
        return h.ComputeHash(s);
    }

    private static void Fail()
    {
        Console.WriteLine("Unknown error.");
        Environment.Exit(1);
    }
}