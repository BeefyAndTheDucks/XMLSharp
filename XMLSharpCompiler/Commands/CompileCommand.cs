using Common;

using System.CommandLine;
using System.Diagnostics;

namespace XMLSharpCompiler.Commands;

public class CompileCommand : CommandBase
{
    private readonly Argument<FileInfo> _fileArg = new("file");
    private readonly Option<FileInfo> _outputFileArg = new("output");
    
    private readonly Option<bool> _verboseArg = new("--verbose", "-v");

    protected override void Invoke(ParseResult parseResult)
    {
        FileInfo inputFile = parseResult.GetRequiredValue(_fileArg);
        if (!inputFile.Exists)
        {
            Console.Error.WriteLine($"File {inputFile.FullName} does not exist.");
            Environment.Exit(1);
        }
        
        FileInfo outputFile = parseResult.GetValue(_outputFileArg) ?? new FileInfo(
            Path.Combine(inputFile.Directory!.FullName, Path.GetFileNameWithoutExtension(inputFile.Name) + ".ir"));

        bool verbose = parseResult.GetValue(_verboseArg);

        Console.WriteLine($"Compiling {inputFile.FullName}...");
        
        Stopwatch sw = Stopwatch.StartNew();
        ILexer lexer = new Lexer();
        IAstGenerator astGenerator = new AstGenerator();
        SyntaxValidator validator = new();

        Token[] tokens = lexer.Lex(File.ReadAllText(inputFile.FullName));
        
        if (verbose)
            tokens.PrettyPrint();
        
        SyntaxError[] errors = validator.Validate(tokens);

        if (errors.Length > 0)
        {
            foreach (SyntaxError error in errors)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.Write($"Syntax error at {error.Line}:{error.Col} — {error.Message}");
                Console.ResetColor();
                Console.Error.WriteLine();
            }
            string s = errors.Length != 0 ? "s" : "";
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Error.Write($"{errors.Length} syntax error{s} found. Compilation aborted.");
            Console.ResetColor();
            Console.Error.WriteLine();
            Environment.Exit(1);
        }
        
        AstNode ast = astGenerator.Generate(tokens);
        
        if (verbose)
            Console.WriteLine(ast);
        
        IIR irGenerator = new IR();
        IRInstruction[] instructions = irGenerator.FromAst(ast);

        if (verbose)
            instructions.PrettyPrint();
        
        long compileTime = sw.ElapsedMilliseconds;
        
        Console.WriteLine("Compilation finished, writing IR to file...");
        
        irGenerator.WriteToFile(outputFile, instructions);
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Compilation completed in {compileTime}ms. Results saved to {outputFile.FullName} in {sw.ElapsedMilliseconds - compileTime}ms.");
        Console.ResetColor();
    }

    protected override Command GetCommand()
    {
        return new Command("compile", "Compile a XMLSharp file to IR which can be executed by the interpreter")
        {
            _fileArg,
            _outputFileArg,
            _verboseArg
        };
    }
}