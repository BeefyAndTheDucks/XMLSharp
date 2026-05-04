namespace XMLSharpCompiler;

public record SyntaxError(string Message, int Line, int Col, int Length);