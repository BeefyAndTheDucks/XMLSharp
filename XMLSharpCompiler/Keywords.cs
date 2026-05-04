using Common;

namespace XMLSharpCompiler;

public static class Keywords
{
    public static readonly Dictionary<string, Func<Token>> Map = new()
    {
        { "number", () => new VariableDefinitionToken(XMLSType.Number) },
        { "yesno",  () => new VariableDefinitionToken(XMLSType.Bool) },
        { "decimal", () => new VariableDefinitionToken(XMLSType.Float) },
        { "text", () => new VariableDefinitionToken(XMLSType.Text)},
        { "write", () => new PrintToken() },
        { "yes", () => new YesToken() },
        { "no", () => new NoToken() },
        
        { "if", () => new IfToken() },
        { "else", () => new ElseToken() },
    };
}