using Common;

namespace XMLSharpCompiler;

public abstract record Token(int Line = 0, int Col = 0);

// matched by content
public record NumberToken(int Value, int Line = 0, int Col = 0) : Token(Line, Col);
public record TextToken(string Text, int Line = 0, int Col = 0) : Token(Line, Col);
public record IdentifierToken(string Name, int Line = 0, int Col = 0) : Token(Line, Col);
public record VariableDefinitionToken(XMLSType Type, int Line = 0, int Col = 0) : Token(Line, Col);

public record IfToken(int Line = 0, int Col = 0) : Token(Line, Col);

// matched by pattern
public record AssignmentToken(int Line = 0, int Col = 0) : Token(Line, Col);

public record AddToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record SubtractToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record MultiplyToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record DivideToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record ModuloToken(int Line = 0, int Col = 0) : Token(Line, Col);

public record LessOrEqualsToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record GreaterOrEqualsToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record GreaterToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record LessToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record EqualsToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record NotEqualsToken(int Line = 0, int Col = 0) : Token(Line, Col);

public record AndToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record OrToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record NotToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record XorToken(int Line = 0, int Col = 0) : Token(Line, Col);

public record YesToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record NoToken(int Line = 0, int Col = 0) : Token(Line, Col);

public record ConcatToken(int Line = 0, int Col = 0) : Token(Line, Col);

public record OpenParenToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record CloseParenToken(int Line = 0, int Col = 0) : Token(Line, Col);

public record SemicolonToken(int Line = 0, int Col = 0) : Token(Line, Col);

public record BeginBlockToken(int Line = 0, int Col = 0) : Token(Line, Col);
public record EndBlockToken(int Line = 0, int Col = 0) : Token(Line, Col);

public record PrintToken(int Line = 0, int Col = 0) : Token(Line, Col);

// special
public record EOFToken(int Line = 0, int Col = 0) : Token(Line, Col);