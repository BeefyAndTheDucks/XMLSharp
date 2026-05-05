using Common;

namespace XMLSharpCompiler;

public partial class SyntaxValidator
{
    private Token[] tokens = [];
    private int pos;
    private readonly List<Diagnostic> errors = [];

    private Token Current => tokens[pos];
    private Token Peek(int offset = 1) => tokens[Math.Min(pos + offset, tokens.Length - 1)];

    private Token Advance()
    {
        Token t = Current;
        if (Current is not EOFToken) pos++;
        return t;
    }

    private bool Check<T>() where T : Token => Current is T;

    private bool Match<T>() where T : Token
    {
        if (!Check<T>()) return false;
        Advance();
        return true;
    }

    private void Expect<T>(string message) where T : Token
    {
        if (Current is T) { Advance(); return; }
        Token lastToken = pos > 0 ? tokens[pos - 1] : Current;
        errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, message, lastToken.Line, lastToken.Col, lastToken.Length));
        if (typeof(T) == typeof(SemicolonToken))
            Synchronise();
    }

    public Diagnostic[] Validate(Token[] input)
    {
        tokens = input;
        pos = 0;
        errors.Clear();

        while (Current is not EOFToken)
            ValidateStatement();

        HashSet<(int Line, int Col)> seen = [];
        return errors
            .OrderBy(e => e.Line)
            .ThenBy(e => e.Col)
            .Where(e => seen.Add((e.Line, e.Col)))
            .ToArray();
    }

    private string TokenName(Token token)
    {
        if (token is IdentifierToken id) return $"'{id.Name}'";
        if (token is TypeToken type) return type.Type switch
        {
            XMLSType.Number => "'number'",
            XMLSType.Bool => "'yesno'",
            XMLSType.Float => "'decimal'",
            XMLSType.Text => "'text'",
            _ => "'type'"
        };
        return TokenReverseMap.TryGet(token, out string value) ? $"'{value}'" : token.GetType().Name.Replace("Token", "");
    }
}