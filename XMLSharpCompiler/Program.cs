using System.Diagnostics;

namespace XMLSharpCompiler;

class Program
{
    static void Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();
        
        ILexer lexer = new Lexer();
        IAstGenerator astGenerator = new AstGenerator();

        Token[] tokens = lexer.Lex("number foo = 2;");
        
        AstNode ast = astGenerator.Generate(tokens);
        
        Console.WriteLine($"Compilation completed in {sw.ElapsedMilliseconds}ms. Result: {ast}");
    }
}