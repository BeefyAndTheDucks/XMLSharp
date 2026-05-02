namespace XMLSharpCompiler;

public class AstGenerator : IAstGenerator
{
    public AstNode Generate(Token[] tokens)
    {
        return Parse(tokens);
    }
    
    private static AstNode Parse(Token[] tokens, int currentIndex = 0)
    {
        Token currentToken = tokens[currentIndex];

        switch (currentToken)
        {
            case VariableDefinitionToken variableDefinitionToken:
            {
                currentIndex++;
                IdentifierToken identifierToken = ConvertOrThrow<IdentifierToken>(tokens[currentIndex++]);
                ConvertOrThrow<AssignmentToken>(tokens[currentIndex++]);
                return new CreateVariableNode(identifierToken.Name, variableDefinitionToken.Type, Parse(tokens, currentIndex));
            }
            case ImmediateToken immediateToken:
            {
                currentIndex++;
                Token nextToken = tokens[currentIndex];
                NumberNode self = new(immediateToken.Value);
                switch (nextToken)
                {
                    case AddToken:
                        currentIndex++;
                        return new AddNode(self, Parse(tokens, currentIndex));
                }
                return self;
            }
            case IdentifierToken identifierToken:
            {
                currentIndex++;
                Token nextToken = tokens[currentIndex];

                if (nextToken is AssignmentToken)
                    return new SetVariableNode(identifierToken.Name, Parse(tokens, currentIndex + 1));
                
                return new GetVariableNode(identifierToken.Name);
            }
        }
        
        throw new UnexpectedTokenException(currentToken);
    }

    private static T ConvertOrThrow<T>(Token token)
    {
        if (token is T t) return t;
        throw new UnexpectedTokenException(token);
    }
}

public class UnexpectedTokenException(Token token) : Exception($"Unexpected token: {token}.");
