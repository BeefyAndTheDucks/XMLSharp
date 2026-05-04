using static XMLSharpCompiler.TokenFollowers;

namespace XMLSharpCompiler;

// remember to add new rules to SyntaxRules!
public static class SyntaxRules
{
    public static readonly ISyntaxRule[] All = [
        new VariableDeclarationRule(),
        new UnexpectedTokenRule(),
    ];
}

// rule for variable declaration
public class VariableDeclarationRule : ITokenRule
{
    public SyntaxError? Validate(Token[] tokens, int index)
    {
        if (tokens[index] is not VariableDefinitionToken) return null;

        if (index + 1 >= tokens.Length || tokens[index + 1] is not IdentifierToken identifierToken)
            return new SyntaxError("Expected identifier after type.", tokens[index].Line, tokens[index].Col);

        if (index + 2 >= tokens.Length || tokens[index + 2] is not AssignmentToken)
            return new SyntaxError($"Expected '=' after '{identifierToken.Name}'.", tokens[index + 1].Line, tokens[index + 1].Col);

        return null;
    }
}

// rule for unexpected tokens
public class UnexpectedTokenRule : ITokenRule
{
    public SyntaxError? Validate(Token[] statement, int index)
    {   

        Token current = statement[index];
        if (!ValidFollowers.TryGetValue(current.GetType(), out HashSet<Type>? followers)) return null;
        if (index + 1 >= statement.Length) return null;

        Token next = statement[index + 1];
        if (followers.Contains(next.GetType())) return null;

        if (next is IdentifierToken or VariableDefinitionToken)
            return new SyntaxError("Missing ';' after statement.", current.Line, current.Col);

        return new SyntaxError($"Unexpected token '{next.GetType().Name}'.", next.Line, next.Col);
    }
}