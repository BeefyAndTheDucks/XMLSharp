namespace XMLSharpCompiler;

public static class Definitions
{
    // sort longest first or the lexer wont lex
    private static readonly (string Pattern, Func<Token> Create)[] Map = [
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
        
        // Text
        ("#"   , () => new ConcatToken()),
        
        // Other
        ("="   , () => new AssignmentToken()),
        ("("   , () => new OpenParenToken()),
        (")"   , () => new CloseParenToken()),
        (";"   , () => new SemicolonToken()),
    ];

    public static (string Pattern, Func<Token> Create)[] MatchingMap
    {
        get
        {
            if (field is not null) return field;
            field = Map;
            field.Sort((a, b) => b.Pattern.Length.CompareTo(a.Pattern.Length));
            return field;
        }
    }
}