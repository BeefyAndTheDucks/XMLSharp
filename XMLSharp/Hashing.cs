using System.Security.Cryptography;

namespace XMLSharp;

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
        string baseDir = AppContext.BaseDirectory;
        string f = Path.Combine(baseDir, "important", "xavier.txt");

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