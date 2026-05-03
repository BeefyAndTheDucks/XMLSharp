using Common;

using System.CommandLine;
using System.Diagnostics;

namespace XMLSharpCompiler.Commands;

public class CompileCommand : CommandBase
{
    private readonly Argument<FileInfo> _fileArg = new("file");
    private readonly Option<FileInfo> _outputFileArg = new("output");

    protected override void Invoke(ParseResult parseResult)
    {
        FileInfo inputFile = parseResult.GetRequiredValue(_fileArg);
        if (!inputFile.Exists)
            Console.Error.WriteLine($"File {inputFile.FullName} does not exist.");
        
        FileInfo outputFile = parseResult.GetValue(_outputFileArg) ?? new FileInfo(
            Path.Combine(inputFile.Directory!.FullName, Path.GetFileNameWithoutExtension(inputFile.Name) + ".ir"));

        Console.WriteLine($"Compiling {inputFile.FullName}...");
        
        Stopwatch sw = Stopwatch.StartNew();
        ILexer lexer = new Lexer();
        IAstGenerator astGenerator = new AstGenerator();
        SyntaxValidator validator = new();

        Token[] tokens = lexer.Lex(File.ReadAllText(inputFile.FullName));
        SyntaxError[] errors = validator.Validate(tokens);

        int errorCount = 0;
        if (errors.Length > 0)
        {
            foreach (SyntaxError error in errors)
            {
                errorCount++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Syntax error at {error.Line}:{error.Col} — {error.Message}");
                Console.ResetColor();
            }
            string s = (errorCount != 0) ? "s" : "";
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Error.WriteLine($"{errorCount} syntax error{s} found. Compilation aborted.");
            Console.ResetColor();
            Environment.Exit(1);
        }
        
        AstNode[] ast = astGenerator.Generate(tokens);
        
        IIR irGenerator = new IR();
        IRInstruction[] instructions = irGenerator.FromAst(ast);
        
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
            _outputFileArg
        };
    }
}