namespace XMLSharpCompiler;

class Program
{
    static void Main(string[] args)
    {
        ILexer lexer = new Lexer();

        Token[] tokens = lexer.Lex();
        
        Console.WriteLine("Goodbye sanity =)");
    }
}