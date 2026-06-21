using Common;

namespace XMLSharpCompiler;

public abstract record Token(Location? Location = null);

// matched by content
public record NumberToken(int Value, Location? Location = null) : Token(Location);
public record TextToken(string Text, Location? Location = null) : Token(Location);
public record IdentifierToken(string Name, Location? Location = null) : Token(Location);
public record TypeToken(XMLSType Type, Location? Location = null) : Token(Location);
public record DecimalToken(float Value, Location? Location = null) : Token(Location);

public record IfToken(Location? Location = null) : Token(Location);
public record ElseToken(Location? Location = null) : Token(Location);
public record ElifToken(Location? Location = null) : Token(Location);

public record WhileToken(Location? Location = null) : Token(Location);
public record ForToken(Location? Location = null) : Token(Location);

public record FunctionToken(Location? Location = null) : Token(Location);
public record ReturnToken(Location? Location = null) : Token(Location);
public record SeparatorToken(Location? Location = null) : Token(Location);

public record ImportToken(Location? Location = null) : Token(Location);

// matched by pattern
public record AssignmentToken(Location? Location = null) : Token(Location);

public record AddToken(Location? Location = null) : Token(Location);
public record SubtractToken(Location? Location = null) : Token(Location);
public record MultiplyToken(Location? Location = null) : Token(Location);
public record DivideToken(Location? Location = null) : Token(Location);
public record ModuloToken(Location? Location = null) : Token(Location);

public record LessOrEqualsToken(Location? Location = null) : Token(Location);
public record GreaterOrEqualsToken(Location? Location = null) : Token(Location);
public record GreaterToken(Location? Location = null) : Token(Location);
public record LessToken(Location? Location = null) : Token(Location);
public record EqualsToken(Location? Location = null) : Token(Location);
public record NotEqualsToken(Location? Location = null) : Token(Location);

public record AndToken(Location? Location = null) : Token(Location);
public record OrToken(Location? Location = null) : Token(Location);
public record NotToken(Location? Location = null) : Token(Location);
public record XorToken(Location? Location = null) : Token(Location);

public record YesToken(Location? Location = null) : Token(Location);
public record NoToken(Location? Location = null) : Token(Location);

public record ConcatToken(Location? Location = null) : Token(Location);

public record OpenParenToken(Location? Location = null) : Token(Location);
public record CloseParenToken(Location? Location = null) : Token(Location);

public record SemicolonToken(Location? Location = null) : Token(Location);

public record BeginBlockToken(Location? Location = null) : Token(Location);
public record EndBlockToken(Location? Location = null) : Token(Location);

public record PrintToken(Location? Location = null) : Token(Location);

public record IncrementByToken(Location? Location = null) : Token(Location);
public record DecrementByToken(Location? Location = null) : Token(Location);
public record MultiplyByToken(Location? Location = null) : Token(Location);
public record DivideByToken(Location? Location = null) : Token(Location);
public record ModuloByToken(Location? Location = null) : Token(Location);
public record IncrementToken(Location? Location = null) : Token(Location);
public record DecrementToken(Location? Location = null) : Token(Location);

// special
public record EOFToken(Location? Location = null) : Token(Location);