using System.CommandLine;
using XMLSharpInterpreter.Commands;

namespace XMLSharpInterpreter;

internal static class Program
{
    private static void Main(string[] args)
    {
        RootCommand rootCommand = new()
        {
            new RunCommand().CreateCommand()
        };

        rootCommand.Parse(args)
            .Invoke();
    }
}