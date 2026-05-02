namespace XMLSharpCompiler;

public static class Definitions
{

    // sort longest first or the lexer wont lex
    public static readonly (string Pattern, Func<Token> Create, int Length)[] Map = [
        ("=<", () => new LessOrEqualsToken(), 2),
        ("=>", () => new GreaterOrEqualsToken(), 2),
        ("====", () => new EqualsToken(), 4),
        ("=",  () => new AssignmentToken(), 1),
        ("+",  () => new AddToken(), 1),
        (";",  () => new SemicolonToken(), 1),
    ];
}