using System.CommandLine;
using System.Diagnostics;
using XMLSharpCompiler;
using Version = XMLSharpCompiler.Version;

namespace Client.Commands;

public class CompileCommand : CommandBase
{
    private readonly Option<FileInfo> _fileOption = new("--file", "file", "-f", "--input", "-i", "input");
    
    protected override void Invoke(ParseResult parseResult)
    {
        FileInfo file = parseResult.GetRequiredValue(_fileOption);
        
        Console.WriteLine($"Compiling {file.FullName}...");
        
        Stopwatch sw = Stopwatch.StartNew();
        ILexer lexer = new Lexer();
        IAstGenerator astGenerator = new AstGenerator();

        Token[] tokens = lexer.Lex(File.ReadAllText(file.FullName));
        
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
            _fileOption
        };
    }
}