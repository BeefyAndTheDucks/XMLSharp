using System.CommandLine;
using System.Diagnostics;
using XMLSharpCompiler;
using Version = XMLSharpCompiler.Version;

namespace Client.Commands;

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
        if (errors.Length > 0)
        {
            foreach (SyntaxError error in errors)
            {
                Console.WriteLine($"Syntax error at {error.Line}:{error.Col} — {error.Message}");
            }
            return;
        }
        
        AstNode[] ast = astGenerator.Generate(tokens);
        
        Console.WriteLine($"Compilation completed in {sw.ElapsedMilliseconds}ms. Printing results...");

        for (int i = 0; i < ast.Length; i++)
        {
            Console.WriteLine($"ast[{i}] = {ast[i]}");
        }
    }

    protected override Command GetCommand()
    {
        return new Command("compile", "Compile a XMLSharp file to IR which can be executed by the interpreter")
        {
            _fileArg
        };
    }
}