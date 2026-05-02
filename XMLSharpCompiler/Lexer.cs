namespace XMLSharpCompiler;

// tokens
public abstract class Token { }

public class IdentifierToken(string name) : Token
{
    public string Name = name;
}

public class ImmediateToken(int value) : Token
{
    public int Value = value;
}

public class AssignmentToken : Token { }
public class AddToken : Token { }
public class SemicolonToken : Token { }

// actual code
public class Lexer : ILexer
{
    private static readonly TokenDefinition[] Definitions = {

    };

    public Token[] Lex(string input)
    {
        List<Token> tokens = [];

        int i = 0;

    }
}