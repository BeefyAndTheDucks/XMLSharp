namespace Common;

public enum XMLSErrorType
{
    SyntaxError,
    TypeError,
    NameError,
    Warning

}
public record Diagnostic(XMLSErrorType Type, string Message, int Line, int Col, int Length);