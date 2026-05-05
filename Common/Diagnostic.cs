namespace Common;

public enum XMLSErrorType
{
    SyntaxError,
    TypeError,
    NameError

}
public record Diagnostic(XMLSErrorType Type, string Message, int Line, int Col, int Length);