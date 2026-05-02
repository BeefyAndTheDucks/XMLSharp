using System.Diagnostics;

namespace XMLSharpCompiler;

class Program
{
    static void Main(string[] args)
    {
        string version = Version.GetExecutableVersion();
        Console.WriteLine($"Starting XML# Compiler {version}");

        Stopwatch sw = Stopwatch.StartNew();
        Hashing.TamperProtection();
        
        ILexer lexer = new Lexer();
        IAstGenerator astGenerator = new AstGenerator();

        Token[] tokens = lexer.Lex("number foo = 2;");
        
        AstNode ast = astGenerator.Generate(tokens);
        
        Console.WriteLine($"Compilation completed in {sw.ElapsedMilliseconds}ms. Result: {ast}");
    }
}