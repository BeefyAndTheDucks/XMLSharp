using System.CommandLine;
using XMLSharpInterpreter.Commands;
using Common;

namespace XMLSharpInterpreter;

internal static class Program
{
    private static void Main(string[] args)
    {
        Hashing.TamperProtection();
        RootCommand rootCommand = new()
        {
            new RunCommand().CreateCommand()
        };

        rootCommand.Parse(args)
            .Invoke();
    }
}