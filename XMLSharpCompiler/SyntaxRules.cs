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

// rule for semicolon and unexpected tokens 
public class UnexpectedTokenRule : ITokenRule
{
    private static readonly HashSet<Type> ValidFollowers = new()
    {
        // valid followers for tokens in ValueTokens
        typeof(SemicolonToken),
        typeof(AddToken),
        typeof(SubtractToken),
        typeof(MultiplyToken),
        typeof(DivideToken),
        typeof(ModuloToken),
        typeof(EqualsToken),
        typeof(NotEqualsToken),
        typeof(GreaterToken),
        typeof(LessToken),
        typeof(GreaterOrEqualsToken),
        typeof(LessOrEqualsToken),
        typeof(AndToken),
        typeof(OrToken),
        typeof(XorToken),
        typeof(ConcatToken),
        typeof(AssignmentToken),
        
        typeof(TextToken),
        typeof(NumberToken),
        typeof(YesToken),
        typeof(NoToken),
    };

    private static readonly HashSet<Type> ValueTokens = new()
    {
        typeof(NumberToken),
        typeof(IdentifierToken),
        typeof(YesToken),
        typeof(NoToken),
        typeof(TextToken),
        
        // Functions
        typeof(PrintToken),
    };

    public SyntaxError? Validate(Token[] statement, int index)
    {
        if (!ValueTokens.Contains(statement[index].GetType())) return null;
        if (index + 1 >= statement.Length) return null;

        Token next = statement[index + 1];
        if (ValidFollowers.Contains(next.GetType())) return null;

        if (next is IdentifierToken or VariableDefinitionToken)
        {
            return new SyntaxError("Missing ';' after statement.", statement[index].Line, statement[index].Col);
        }

        return new SyntaxError($"Unexpected token '{next.GetType().Name}'.", next.Line, next.Col);
    }
}