using Common;

namespace XMLSharpCompiler;

public class Lexer : ILexer
{
    private readonly List<Diagnostic> _errors = [];

    public (Token[] Tokens, Diagnostic[] Errors) Lex(string input)
    {
        List<Token> tokens = [];
        _errors.Clear();

        var keywords = Keywords.Map;
        var definitions = Definitions.MatchingMap;

        int line = 1;
        int col = 1;
        int i = 0;

        while (i < input.Length)
        {
            if (input[i] == '\n')
            {
                line++;
                col = 1;
                i++;
                continue;
            }

            // whitespace skipping
            if (input[i] == ' ')
            {
                i++;
                col++;
                continue;
            }

            // comment skipping
            if (input[i] == '$')
            {
                while (i < input.Length && input[i] != '\n')
                {
                    i++;
                    col++;
                }
                continue;
            }

            // digit handling
            if (char.IsDigit(input[i]) || input[i] == '.')
            {
                string word = "";
                int startCol = col;
                bool hasDot = false;

                while (i < input.Length && (char.IsDigit(input[i]) || input[i] == '.'))
                {
                    if (input[i] == '.')
                    {
                        if (hasDot) break;
                        hasDot = true;
                    }

                    word += input[i];
                    i++;
                    col++;
                }
                int length = col - startCol;

                if (hasDot)
                    tokens.Add(new DecimalToken(float.Parse(word), line, startCol, length));
                else
                    tokens.Add(new NumberToken(int.Parse(word), line, startCol, length));

                continue;
            }

            // letter handling
            if (char.IsLetter(input[i]) || input[i] == '_')
            {
                string word = "";
                int startCol = col;
                while (i < input.Length && (char.IsLetterOrDigit(input[i]) || input[i] == '_'))
                {
                    word += input[i];
                    i++;
                    col++;
                }
                int length = col - startCol;

                if (keywords.TryGetValue(word, out var create))
                {
                    Token token = create();
                    token = token with { Line = line, Col = startCol, Length = length };
                    tokens.Add(token);
                }
                else
                {
                    tokens.Add(new IdentifierToken(word, line, startCol, length));
                }
                continue;
            }

            // string handling
            if (input[i] == '"')
            {
                int startCol = col;
                i++;
                col++;
                string word = "";

                while (i < input.Length && input[i] != '"')
                {
                    if (input[i] == ';')
                    {
                        _errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Unterminated string.", line, startCol, col - startCol));
                        break;
                    }

                    if (input[i] == '\\')
                    {
                        i++;
                        col++;
                        if (i >= input.Length) break;

                        word += input[i] switch
                        {
                            'n' => '\n',
                            't' => '\t',
                            '"' => '"',
                            ';' => ';',
                            '\\' => '\\',
                            var c => c
                        };
                    }
                    else
                    {
                        word += input[i];
                    }

                    i++;
                    col++;
                }

                if (i >= input.Length)
                {
                    _errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Unterminated string.", line, startCol, col - startCol));
                    continue;
                }

                i++;
                col++;
                int length = col - startCol;
                tokens.Add(new TextToken(word, line, startCol, length));
                continue;
            }

            // pattern matching
            bool matched = false;
            foreach ((string Pattern, Func<Token> Create) definition in definitions)
            {
                int length = definition.Pattern.Length;
                if (i + length <= input.Length && input.Substring(i, length) == definition.Pattern)
                {
                    Token token = definition.Create();
                    token = token with { Line = line, Col = col, Length = length };
                    tokens.Add(token);
                    i += length;
                    col += length;
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                _errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, $"Unexpected character '{input[i]}'.", line, col, 1));
                i++;
                col++;
            }
        }

        tokens.Add(new EOFToken(line, col));
        return (tokens.ToArray(), _errors.ToArray());
    }
}