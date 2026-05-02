namespace XMLSharpCompiler;

public abstract record Token;

// matched by content
public record NumberToken(int Value) : Token;
public record TextToken(string Text) : Token;
public record IdentifierToken(string Name) : Token;
public record VariableDefinitionToken(XMLSType Type) : Token;

// matched by pattern
public record AssignmentToken : Token;

public record AddToken : Token;
public record SubtractToken : Token;
public record MultiplyToken : Token;
public record DivideToken : Token;
public record ModuloToken : Token;

public record LessOrEqualsToken : Token;
public record GreaterOrEqualsToken : Token;
public record GreaterToken : Token;
public record LessToken : Token;
public record EqualsToken : Token;
public record NotEqualsToken : Token;

public record SemicolonToken : Token;

public record AndToken : Token;
public record OrToken : Token;
public record NotToken : Token;
public record XorToken : Token;

public record YesToken : Token;
public record NoToken : Token;

// special
public record EOFToken : Token;