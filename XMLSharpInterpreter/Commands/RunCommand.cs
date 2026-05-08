using System.CommandLine;
using System.Text.Json;
using Common;

namespace XMLSharpInterpreter.Commands;

public class RunCommand : CommandBase
{
    private readonly Argument<FileInfo> _fileArg = new("file");
    private readonly Option<bool> _verboseArg = new("--verbose", "-verbose", "-v", "--v");
    
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

        try {
            IRInstruction[]? instructions = ir.ReadFromFile(inputFile);

            if (instructions is null || instructions.Length == 0)
            {
                Console.Error.WriteLine($"{inputFile.Name} is empty or malformed.");
                Environment.Exit(1);
            }
            Interpreter interpreter = new();
            interpreter.Run(instructions, parseResult.GetValue(_verboseArg));
        } catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"An error occured while running {inputFile.Name}: {e.Message}\n{e.StackTrace}");
            Console.ResetColor();
        }
    }

    protected override Command GetCommand()
    {
        return new Command("run", "Run a compiled XMLSharp file")
        {
            _fileArg,
            _verboseArg
        };
    }
}