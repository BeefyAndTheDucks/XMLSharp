namespace Common;

public enum XMLSErrorType
{
    SyntaxError,
    TypeError,
    NameError,
    Warning
}

public record Diagnostic(XMLSErrorType Type, string Message, Location? Location)
{
    public static Diagnostic SyntaxError(string message, Location? location) => new(XMLSErrorType.SyntaxError, message, location);
    public static Diagnostic TypeError(string message, Location? location) => new(XMLSErrorType.TypeError, message, location);
    public static Diagnostic NameError(string message, Location? location) => new(XMLSErrorType.NameError, message, location);
    public static Diagnostic Warning(string message, Location? location) => new(XMLSErrorType.Warning, message, location);
}
