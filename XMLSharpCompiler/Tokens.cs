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
public record LessOrEqualsToken : Token;
public record GreaterOrEqualsToken : Token;
public record EqualsToken : Token;
public record SemicolonToken : Token;

// special
public record EOFToken : Token;