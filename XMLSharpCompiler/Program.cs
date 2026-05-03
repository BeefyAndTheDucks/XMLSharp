using System.CommandLine;
using Common;
using XMLSharpCompiler.Commands;

namespace XMLSharpCompiler;

internal static class Program
{
    private static void Main(string[] args)
    {
        Hashing.TamperProtection();
        
        string version = Version.GetExecutableVersion();
        Console.WriteLine($"Starting XMLSharp Compiler v{version}...");
        
        RootCommand rootCommand = new()
        {
            new CompileCommand().CreateCommand()
        };

        rootCommand.Parse(args)
            .Invoke();
    }
}