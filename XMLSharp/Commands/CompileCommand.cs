using System.CommandLine;
using XMLSharpCompiler;

namespace XMLSharp.Commands;

public class CompileCommand : CommandBase
{
    private readonly Argument<FileInfo> _fileArg = new("file");
    private readonly Option<FileInfo> _outputFileArg = new("output");
    
    private readonly Option<bool> _verboseArg = new("--verbose", "-verbose", "-v", "--v");
    private readonly Option<bool> _ignoreErrorsArg = new("--ignore-errors", "-i", "-ignore-errs", "-ierr");

    protected override void Invoke(ParseResult parseResult)
    {
        FileInfo inputFile = parseResult.GetRequiredValue(_fileArg);
        if (!inputFile.Exists)
        {
            Console.Error.WriteLine($"File {inputFile.FullName} does not exist.");
            Environment.Exit(1);
        }

        FileInfo outputFile = parseResult.GetValue(_outputFileArg) ?? Helpers.GetIRFileForSourceFile(inputFile);

        bool verbose = parseResult.GetValue(_verboseArg);
        
        bool ignoreErrors = parseResult.GetValue(_ignoreErrorsArg);

        CompilationSettings compilationSettings = new()
        {
            InputFile = inputFile,
            OutputFile = outputFile,
            
            VerboseMode = verbose,
            IgnoreErrors = ignoreErrors
        };
        
        Compiler.Compile(compilationSettings);
    }

    protected override Command GetCommand()
    {
        return new Command("compile", "Compile a XMLSharp file to IR which can be executed by the interpreter")
        {
            _fileArg,
            _outputFileArg,
            _verboseArg,
            _ignoreErrorsArg
        };
    }
}