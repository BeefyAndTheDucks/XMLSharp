using Common;

namespace XMLSharpCompiler;

public interface ILexer
{
    (Token[] Tokens, Diagnostic[] Errors) Lex(string input, FileInfo? file);
}

