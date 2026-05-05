using Common;

namespace XMLSharpCompiler;

public static class Keywords
{
    public static readonly Dictionary<string, Func<Token>> Map = new()
    {
        { "number", () => new TypeToken(XMLSType.Number) },
        { "yesno",  () => new TypeToken(XMLSType.Bool) },
        { "decimal", () => new TypeToken(XMLSType.Float) },
        { "text", () => new TypeToken(XMLSType.Text)},
        { "write", () => new PrintToken() },
        { "yes", () => new YesToken() },
        { "no", () => new NoToken() },

        { "if", () => new IfToken() },
        { "else", () => new ElseToken() },
        { "elif", () => new ElifToken() },
        
        { "while", () => new WhileToken() },
        { "for", () => new ForToken() },
    };
}