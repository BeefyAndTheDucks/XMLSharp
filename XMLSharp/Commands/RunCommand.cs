using System.CommandLine;
using XMLSharpCompiler;
using XMLSharpInterpreter;

namespace XMLSharp.Commands;

public class RunCommand : CommandBase
{
    private readonly Argument<FileInfo> _fileArg = new("file");
    private readonly Option<bool> _verboseArg = new("--verbose", "-verbose", "-v", "--v");
    private readonly Option<bool> _debuggerArg = new("--debugger", "-debugger", "--debug", "-debug", "-d", "--d");
    private readonly Option<int> _debuggerAutoStepRateArg = new("--debugger-auto-step-rate-seconds", "--auto-step", "--step-rate");
    
    protected override void Invoke(ParseResult parseResult)
    {
        FileInfo inputFile = parseResult.GetRequiredValue(_fileArg);
        if (!inputFile.Exists)
        {
            Console.Error.WriteLine($"File {inputFile.FullName} does not exist.");
            Environment.Exit(1);
        }
        
        bool verbose = parseResult.GetValue(_verboseArg);
        bool debug = parseResult.GetValue(_debuggerArg);

        int stepRate = parseResult.GetValue(_debuggerAutoStepRateArg);
        TimeSpan? autoStepRate = stepRate > 0 ? TimeSpan.FromSeconds(stepRate) : null;

        if (!Interpreter.CanInterpret(inputFile))
        {
            FileInfo irOutput = Helpers.GetIRFileForSourceFile(inputFile);
            
            CompilationSettings compilationSettings = new()
            {
                InputFile = inputFile,
                OutputFile = irOutput,
                VerboseMode = verbose
            };
            
            Compiler.Compile(compilationSettings);

            inputFile = irOutput;
        }
        
        InterpreterSettings settings = new()
        {
            InputFile = inputFile,
            VerboseMode = verbose,
            Debugger = debug,
            
            DebuggerAutoStepRate = autoStepRate
        };

        Interpreter.Interpret(settings);
    }

    protected override Command GetCommand()
    {
        return new Command("run", "Run a compiled XMLSharp file")
        {
            _fileArg,
            _verboseArg,
            _debuggerArg,
            _debuggerAutoStepRateArg
        };
    }
}