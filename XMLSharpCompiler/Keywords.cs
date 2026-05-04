using Common;

namespace XMLSharpCompiler;

public static class Keywords
{
    public static readonly Dictionary<string, Func<Token>> Map = new()
    {
        { "number", () => new VariableDefinitionToken(XMLSType.Number) },
        { "yesno",  () => new VariableDefinitionToken(XMLSType.Bool) },
        { "text", () => new VariableDefinitionToken(XMLSType.Text)},
        { "write", () => new PrintToken() },
        { "yes", () => new YesToken() },
        { "no", () => new NoToken() },
    };
}