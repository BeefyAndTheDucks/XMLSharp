using Common;

namespace XMLSharpCompiler;

public interface ITokenProcessor
{
    (Token[], Diagnostic[]) Process(Token[] tokens, Diagnostic[] diagnostics);
}

public static class TokenProcessorExtensions
{
    extension((Token[] tokens, Diagnostic[] diagnostics) context)
    {
        public (Token[], Diagnostic[]) AddProcessor(ITokenProcessor processor)
        {
            return processor.Process(context.tokens, context.diagnostics);
        }
    }
}