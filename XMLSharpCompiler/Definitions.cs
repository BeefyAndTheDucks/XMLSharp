namespace XMLSharpCompiler;

public static class Definitions
{

    // sort longest first or the lexer wont lex
    public static readonly (string Pattern, Func<Token> Create)[] Map = [
        // Boolean
        ("====", () => new EqualsToken()),
        ("!===", () => new NotEqualsToken()),
        ("=<"  , () => new LessOrEqualsToken()),
        ("=>"  , () => new GreaterOrEqualsToken()),
        ("<"   , () => new LessToken()),
        (">"   , () => new GreaterToken()),
        ("&"   , () => new AndToken()),
        ("?"   , () => new OrToken()),
        ("~"   , () => new NotToken()),
        ("!"   , () => new XorToken()),
        ("yes" , () => new YesToken()),
        ("no"  , () => new NoToken()),
        
        // Numbers
        ("+"   , () => new AddToken()),
        ("-"   , () => new SubtractToken()),
        ("*"   , () => new MultiplyToken()),
        ("/"   , () => new DivideToken()),
        ("%"   , () => new ModuloToken()),
        
        // Other
        ("="   , () => new AssignmentToken()),
        (";"   , () => new SemicolonToken()),
    ];
}