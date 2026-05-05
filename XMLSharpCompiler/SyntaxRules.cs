using static XMLSharpCompiler.TokenFollowers;
using Common;

namespace XMLSharpCompiler;

// remember to add new rules to SyntaxRules!
public static class SyntaxRules
{
    public static readonly ISyntaxRule[] All = [
        new VariableDeclarationRule(),
        new UnexpectedTokenRule(),
        new ParenMatchRule(),
        new BraceMatchRule()
    ];
}

// rule for variable declaration
public class VariableDeclarationRule : ITokenRule
{
    public Diagnostic? Validate(Token[] tokens, int index)
    {
        if (tokens[index] is not TypeToken) return null;

        if (index + 1 >= tokens.Length || tokens[index + 1] is not IdentifierToken identifierToken)
            return new Diagnostic(
                XMLSErrorType.SyntaxError,
                "Expected identifier after type.", 
                tokens[index].Line, 
                tokens[index].Col, 
                tokens[index].Length
                );

        if (index + 2 >= tokens.Length || tokens[index + 2] is not AssignmentToken)
            return new Diagnostic(
                XMLSErrorType.SyntaxError,
                $"Expected '=' after '{identifierToken.Name}'.", 
                tokens[index + 1].Line, 
                tokens[index + 1].Col, 
                tokens[index + 1].Length
            );

        return null;
    }
}

// rule for unexpected tokens
public class UnexpectedTokenRule : ITokenRule
{
    public Diagnostic? Validate(Token[] statement, int index)
    {   
        if (index + 1 >= statement.Length) return null;
        Token current = statement[index];
        Token next = statement[index + 1];

        if (!ValidFollowers.TryGetValue(current.GetType(), out HashSet<Type>? followers)) return null;
        if (followers.Contains(next.GetType())) return null;
        if (next is IdentifierToken or TypeToken or EOFToken)
            return new Diagnostic(XMLSErrorType.SyntaxError, "Missing ';' after statement.", current.Line, current.Col, current.Length);

        return new Diagnostic(XMLSErrorType.SyntaxError, $"Unexpected token '{next.GetType().Name}'.", next.Line, next.Col, next.Length);
    }
}

// rule for matching parens
public class ParenMatchRule : IBlockRule
{
    public Diagnostic[] Validate(Token[] tokens)
    {
        List<Diagnostic> errors = [];
        int depth = 0;
        Token? lastOpen = null;

        foreach (Token token in tokens)
        {
            if (token is OpenParenToken)
            {
                depth++;
                lastOpen = token;
            }
            else if (token is CloseParenToken)
            {
                if (depth == 0)
                {
                    errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Unexpected ')'.", token.Line, token.Col, token.Length));
                }
                else
                {
                    depth--;
                }
            }
        }

        if (depth > 0)
            errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Unclosed '('.", lastOpen!.Line, lastOpen.Col, lastOpen.Length));

        return errors.ToArray();
    }
}

// rule for matching braces.
public class BraceMatchRule : IBlockRule
{
    public Diagnostic[] Validate(Token[] tokens)
    {
        List<Diagnostic> errors = [];
        int depth = 0;
        Token? lastOpen = null;

        foreach (Token token in tokens)
        {
            if (token is BeginBlockToken)
            {
                depth++;
                lastOpen = token;
            }
            else if (token is EndBlockToken)
            {
                if (depth == 0)
                {
                    errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Unexpected '}'.", token.Line, token.Col, token.Length));
                }
                else
                {
                    depth--;
                }
            }
        }

        if (depth > 0)
            errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Unclosed '{'.", lastOpen!.Line, lastOpen.Col, lastOpen.Length));

        return errors.ToArray();
    }
}