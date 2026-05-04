using Common;

namespace XMLSharpCompiler;

public abstract record Token(int Line = 0, int Col = 0, int Length = 0);

// matched by content
public record NumberToken(int Value, int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record TextToken(string Text, int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record IdentifierToken(string Name, int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record VariableDefinitionToken(XMLSType Type, int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record DecimalToken(float Value, int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

public record IfToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record ElseToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record ElifToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

// matched by pattern
public record AssignmentToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

public record AddToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record SubtractToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record MultiplyToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record DivideToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record ModuloToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

public record LessOrEqualsToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record GreaterOrEqualsToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record GreaterToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record LessToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record EqualsToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record NotEqualsToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

public record AndToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record OrToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record NotToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record XorToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

public record YesToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record NoToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

public record ConcatToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

public record OpenParenToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record CloseParenToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

public record SemicolonToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

public record BeginBlockToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);
public record EndBlockToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

public record PrintToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);

// special
public record EOFToken(int Line = 0, int Col = 0, int Length = 0) : Token(Line, Col, Length);