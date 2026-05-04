using System.CommandLine;
using Common;

namespace XMLSharpInterpreter.Commands;

public class RunCommand : CommandBase
{
    private readonly Argument<FileInfo> _fileArg = new("file");
    
    protected override void Invoke(ParseResult parseResult)
    {
        FileInfo inputFile = parseResult.GetRequiredValue(_fileArg);
        if (!inputFile.Exists)
        {
            Console.Error.WriteLine($"File {inputFile.FullName} does not exist.");
            Environment.Exit(1);
        }

        // ir.ReadFromFile will need to return an actual error at some point
        IR ir = new();
        IRInstruction[]? instructions = ir.ReadFromFile(inputFile);
        if (instructions is null)
        {
            Console.Error.WriteLine($"Failed to read from file {inputFile.FullName}");
            Environment.Exit(1);
        }

        Interpreter interpreter = new();
        interpreter.Run(instructions);
    }

    protected override Command GetCommand()
    {
        return new Command("run", "Run a compiled XMLSharp file")
        {
            _fileArg
        };
    }
}