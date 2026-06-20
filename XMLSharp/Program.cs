using System.CommandLine;
using XMLSharp.Commands;

namespace XMLSharp;

internal static class Program
{
    private static void Main(string[] args)
    {
        Hashing.TamperProtection();
        
        string version = Version.GetExecutableVersion();
        Console.WriteLine($"XMLSharp v{version}, Made by BeefyAndTheDucks and meelees");
        
        RootCommand rootCommand = new()
        {
            new CompileCommand().CreateCommand(),
            new RunCommand().CreateCommand()
        };

        rootCommand.Parse(args)
            .Invoke();
    }
}