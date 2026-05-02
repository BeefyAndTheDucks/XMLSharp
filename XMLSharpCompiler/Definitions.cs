namespace XMLSharpCompiler;

public static class Definitions
{

    // sort longest first
    public static readonly (string Pattern, Func<Token> Create, int Length)[] Map = [
        ("<=", () => new LessOrEqualsToken(), 2),
        (">=", () => new GreaterOrEqualsToken(), 2),
        ("=",  () => new AssignmentToken(), 1),
        ("+",  () => new AddToken(), 1),
        (";",  () => new SemicolonToken(), 1),
    ];
}