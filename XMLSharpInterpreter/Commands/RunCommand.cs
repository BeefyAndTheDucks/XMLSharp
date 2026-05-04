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
        IIR ir = new IR();
        IRInstruction[] instructions = ir.ReadFromFile(inputFile);

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