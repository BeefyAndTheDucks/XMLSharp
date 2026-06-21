using Common;

namespace XMLSharpCompiler;

public partial class SyntaxValidator : ITokenProcessor
{
    private Token[] _tokens = [];
    private int _pos;
    private List<Diagnostic> _errors = [];

    private Token Current => _tokens[_pos];
    private Token Previous => Peek(-1);
    private Token Peek(int offset = 1) => _tokens[Math.Clamp(_pos + offset, 0, _tokens.Length - 1)];

    private Token Advance()
    {
        Token t = Current;
        if (Current is not EOFToken) _pos++;
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
        Token lastToken = _pos > 0 ? _tokens[_pos - 1] : Current;
        _errors.Add(Diagnostic.SyntaxError(message, lastToken.Location));
        if (typeof(T) == typeof(SemicolonToken))
            Synchronise();
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

    public (Token[], Diagnostic[]) Process(Token[] input, Diagnostic[] diagnostics)
    {
        _tokens = input;
        _pos = 0;
        _errors = [..diagnostics];

        while (Current is not EOFToken)
            ValidateStatement();

        HashSet<Location?> seen = [];
        return (input, _errors
            .OrderBy(e => e.Location?.Line)
            .ThenBy(e => e.Location?.Column)
            .Where(e => seen.Add(e.Location))
            .ToArray());
    }
}