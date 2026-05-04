namespace XMLSharpCompiler;


// actual code
public class Lexer : ILexer
{

    public Token[] Lex(string input)
    {
        List<Token> tokens = [];

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

                if (hasDot)
                    tokens.Add(new DecimalToken(float.Parse(word), line, startCol));
                else
                    tokens.Add(new NumberToken(int.Parse(word), line, startCol));

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
                if (keywords.TryGetValue(word, out var create))
                {
                    Token token = create();
                    token = token with { Line = line, Col = startCol };
                    tokens.Add(token);
                }
                else
                {
                    tokens.Add(new IdentifierToken(word, line, startCol));
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
                    word += input[i];
                    i++;
                    col++;
                }
                i++;
                col++;
                tokens.Add(new TextToken(word, line, startCol));
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
                    token = token with { Line = line, Col = col };
                    tokens.Add(token);
                    i += length;
                    col += length;
                    matched = true;
                    break;
                }
            }
            if (!matched)
            {
                throw new UnexpectedCharacterException(line, col, input[i]);
            }
        }

        tokens.Add(new EOFToken());
        return tokens.ToArray();
    }
}

public class UnexpectedCharacterException(int line, int col, char character) : Exception($"Unexpected character \"{character}\" at {line}:{col}.")
{
    public readonly int Line = line;
    public readonly int Col = col;
    public readonly char Character = character;
}
