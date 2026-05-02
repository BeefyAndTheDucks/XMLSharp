namespace XMLSharpCompiler;

public interface ILexer
{
    Token[] Lex(string input);
}

