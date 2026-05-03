using Common;

using System.CommandLine;
using System.Diagnostics;

namespace XMLSharpCompiler.Commands;

public class CompileCommand : CommandBase
{
    private readonly Argument<FileInfo> _fileArg = new("file");

    protected override void Invoke(ParseResult parseResult)
    {
        FileInfo? file = parseResult.GetValue(_fileArg);
        if (file is null)
        {
            Console.WriteLine("No file specified for compilation.");
            return;
        } 

        Console.WriteLine($"Compiling {file.FullName}...");
        
        Stopwatch sw = Stopwatch.StartNew();
        ILexer lexer = new Lexer();
        IAstGenerator astGenerator = new AstGenerator();
        SyntaxValidator validator = new();

        Token[] tokens = lexer.Lex(File.ReadAllText(file.FullName));
        SyntaxError[] errors = validator.Validate(tokens);

        int errorCount = 0;
        if (errors.Length > 0)
        {
            foreach (SyntaxError error in errors)
            {
                errorCount++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Syntax error at {error.Line}:{error.Col} — {error.Message}");
                Console.ResetColor();
            }
            string s = (errorCount != 0) ? "s" : "";
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{errorCount} syntax error{s} found. Compilation aborted.");
            Console.ResetColor();
            Environment.Exit(1);
        }
        
        AstNode[] ast = astGenerator.Generate(tokens);

        IIR irGenerator = new IR();
        IRInstruction[] instructions = irGenerator.FromAst(ast);
        
        Console.WriteLine($"Compilation completed in {sw.ElapsedMilliseconds}ms. Printing results...");

        instructions.PrettyPrint();
    }

    protected override Command GetCommand()
    {
        return new Command("compile", "Compile a XMLSharp file to IR which can be executed by the interpreter")
        {
            _fileArg
        };
    }
}