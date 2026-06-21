using Common;

namespace XMLSharpCompiler;

public class ImportExpander(ILexer lexer) : ITokenProcessor
{
    private readonly HashSet<string> _expandedFiles = [];

    public (Token[], Diagnostic[]) Process(Token[] tokens, Diagnostic[] diagnostics)
    {
        List<Token> output = [];
        List<Diagnostic> diagnosticsOut = diagnostics.ToList();

        for (int i = 0; i < tokens.Length; i++)
        {
            Token token = tokens[i];

            if (token is ImportToken)
            {
                i++;
                if (tokens[i] is not TextToken path)
                {
                    diagnosticsOut.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Expected path to import.", token.Line, token.Col, token.Length));
                    continue;
                }

                i++;
                if (tokens[i] is not SemicolonToken)
                {
                    diagnosticsOut.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Expected semicolon.", token.Line, token.Col, token.Length));
                    continue;
                }
                
                FileInfo fileToInclude = new FileInfo(path.Text);
                if (_expandedFiles.Add(fileToInclude.FullName))
                {
                    string contents = File.ReadAllText(fileToInclude.FullName);
                    (Token[] tokens, Diagnostic[] diagnostics) includedTokens = 
                        lexer.Lex(contents)
                            .AddProcessor(this);
                    diagnosticsOut.AddRange(diagnostics);
                    if (includedTokens.tokens[^1] is not EOFToken)
                    {
                        diagnosticsOut.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Expected EOF.", includedTokens.tokens[^1].Line, includedTokens.tokens[^1].Col, includedTokens.tokens[^1].Length));
                        continue;
                    }
                    output.AddRange(includedTokens.tokens[..^1]);
                }
            }
            else
            {
                output.Add(token);
            }
        }
        
        return (output.ToArray(), diagnosticsOut.ToArray());
    }
}