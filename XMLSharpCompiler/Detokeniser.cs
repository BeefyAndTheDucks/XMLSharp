using Common;

namespace XMLSharpCompiler;

public static class Detokeniser
{
    public static string ToSource(Token[] tokens)
    {
        int level = 0;

        string line = "";
        string result = "";

        void FlushLine()
        {
            if (line.Length == 0)
                return;

            result += new string(' ', level * 4) + line.TrimEnd() + "\n";
            line = "";
        }

        foreach (var t in tokens)
        {
            if (t is EndBlockToken)
            {
                FlushLine();
                level--;
                result += new string(' ', level * 4) + "}\n";
                continue;
            }

            string part = t switch
            {
                VariableDefinitionToken v => v.Type switch
                {
                    XMLSType.Number => "number",
                    XMLSType.Bool => "yesno",
                    XMLSType.Float => "decimal",
                    XMLSType.Text => "text",
                    _ => "<unknown>"
                },

                IdentifierToken id => id.Name,
                NumberToken n => n.Value.ToString(),
                DecimalToken d => d.Value.ToString(),
                TextToken txt => $"\"{txt.Text}\"",

                BeginBlockToken => "{",

                _ => TokenReverseMap.TryGet(t, out var mapped)
                    ? mapped
                    : $"<{t.GetType().Name}>"
            };

            if (t is BeginBlockToken)
            {
                FlushLine();
                result += new string(' ', level * 4) + "{\n";
                level++;
                continue;
            }

            line += part + " ";

            if (t is SemicolonToken)
            {
                FlushLine();
            }
        }

        FlushLine();

        return result;
    }
}


public static class TokenReverseMap
{
    private static readonly Dictionary<Type, string> _map = Build();

    private static Dictionary<Type, string> Build()
    {
        var result = new Dictionary<Type, string>();

        foreach (var kv in Keywords.Map)
        {
            Token token = kv.Value();
            result[token.GetType()] = kv.Key;
        }

        foreach (var (pattern, create) in Definitions.MatchingMap)
        {
            Token token = create();
            result[token.GetType()] = pattern;
        }

        return result;
    }

    public static bool TryGet(Token token, out string value)
    {
        return _map.TryGetValue(token.GetType(), out value!);
    }
}