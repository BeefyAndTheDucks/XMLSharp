using static XMLSharpCompiler.TokenFollowers;

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
    public SyntaxError? Validate(Token[] tokens, int index)
    {
        if (tokens[index] is not VariableDefinitionToken) return null;

        if (index + 1 >= tokens.Length || tokens[index + 1] is not IdentifierToken identifierToken)
            return new SyntaxError(
                "Expected identifier after type.", 
                tokens[index].Line, 
                tokens[index].Col, 
                tokens[index].Length
                );

        if (index + 2 >= tokens.Length || tokens[index + 2] is not AssignmentToken)
            return new SyntaxError(
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
    public SyntaxError? Validate(Token[] statement, int index)
    {   
        if (index + 1 >= statement.Length) return null;
        Token current = statement[index];
        Token next = statement[index + 1];

        if (!ValidFollowers.TryGetValue(current.GetType(), out HashSet<Type>? followers)) return null;
        if (followers.Contains(next.GetType())) return null;
        if (next is IdentifierToken or VariableDefinitionToken or EOFToken)
            return new SyntaxError("Missing ';' after statement.", current.Line, current.Col, current.Length);

        return new SyntaxError($"Unexpected token '{next.GetType().Name}'.", next.Line, next.Col, next.Length);
    }
}

// rule for matching parens
public class ParenMatchRule : IBlockRule
{
    public SyntaxError[] Validate(Token[] tokens)
    {
        List<SyntaxError> errors = [];
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
                    errors.Add(new SyntaxError("Unexpected ')'.", token.Line, token.Col, token.Length));
                }
                else
                {
                    depth--;
                }
            }
        }

        if (depth > 0)
            errors.Add(new SyntaxError("Unclosed '('.", lastOpen!.Line, lastOpen.Col, lastOpen.Length));

        return errors.ToArray();
    }
}

// rule for matching braces.
public class BraceMatchRule : IBlockRule
{
    public SyntaxError[] Validate(Token[] tokens)
    {
        List<SyntaxError> errors = [];
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
                    errors.Add(new SyntaxError("Unexpected '}'.", token.Line, token.Col, token.Length));
                }
                else
                {
                    depth--;
                }
            }
        }

        if (depth > 0)
            errors.Add(new SyntaxError("Unclosed '{'.", lastOpen!.Line, lastOpen.Col, lastOpen.Length));

        return errors.ToArray();
    }
}