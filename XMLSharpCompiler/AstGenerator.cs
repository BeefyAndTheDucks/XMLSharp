namespace XMLSharpCompiler;

public class AstGenerator : IAstGenerator
{
    public AstNode[] Generate(Token[] tokens)
    {
        int index = 0;
        List<AstNode> nodes = [];

        while (tokens[index] is not EOFToken)
        {
            nodes.Add(Parse(tokens, index));

            do
            {
                index++;
            } while (tokens[index] is not SemicolonToken or EOFToken);

            index++;
        }

        return nodes.ToArray();
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
            case NumberToken numberToken:
            {
                return ParseNext(tokens, new NumberNode(numberToken.Value), currentIndex);
            }
            case IdentifierToken identifierToken:
            {
                Token nextToken = tokens[currentIndex + 1];
                
                if (nextToken is AssignmentToken)
                    return new SetVariableNode(identifierToken.Name, Parse(tokens, currentIndex + 2));
                
                // Identifier (variables) can be a lot... we must check for them all.
                return ParseNext(tokens, new GetVariableNode(identifierToken.Name), currentIndex);
            }
            case NotToken:
            {
                currentIndex++;
                return new NotNode(Parse(tokens, currentIndex));
            }
            case YesToken:
                return ParseNext(tokens, new BooleanNode(true), currentIndex);
            case NoToken:
                return ParseNext(tokens, new BooleanNode(false), currentIndex);
        }
        
        throw new UnexpectedTokenException(currentToken);
    }

    private static AstNode ParseNext(Token[] tokens, AstNode self, int currentIndex)
    {
        currentIndex++;
        Token nextToken = tokens[currentIndex];
        switch (nextToken)
        {
            // Numbers
            case AddToken:
                currentIndex++;
                return new AddNode(self, Parse(tokens, currentIndex));
            case SubtractToken:
                currentIndex++;
                return new SubtractNode(self, Parse(tokens, currentIndex));
            case MultiplyToken:
                currentIndex++;
                return new MultiplyNode(self, Parse(tokens, currentIndex));
            case DivideToken:
                currentIndex++;
                return new DivideNode(self, Parse(tokens, currentIndex));
            case ModuloToken:
                currentIndex++;
                return new ModuloNode(self, Parse(tokens, currentIndex));
            
            // Comparisons
            case GreaterToken:
                currentIndex++;
                return new GreaterThanNode(self, Parse(tokens, currentIndex));
            case GreaterOrEqualsToken:
                currentIndex++;
                return new GreaterThanOrEqualNode(self, Parse(tokens, currentIndex));
            case LessToken:
                currentIndex++;
                return new LessThanNode(self, Parse(tokens, currentIndex));
            case LessOrEqualsToken:
                currentIndex++;
                return new LessThanOrEqualNode(self, Parse(tokens, currentIndex));
            case EqualsToken:
                currentIndex++;
                return new EqualNode(self, Parse(tokens, currentIndex));
            case NotEqualsToken:
                currentIndex++;
                return new NotEqualNode(self, Parse(tokens, currentIndex));
            
            // Boolean
            case AndToken:
                currentIndex++;
                return new AndNode(self, Parse(tokens, currentIndex));
            case OrToken:
                currentIndex++;
                return new OrNode(self, Parse(tokens, currentIndex));
            case XorToken:
                currentIndex++;
                return new XorNode(self, Parse(tokens, currentIndex));
        }

        return self;
    }

    private static T ConvertOrThrow<T>(Token token)
    {
        if (token is T t) return t;
        throw new UnexpectedTokenException(token);
    }
}

public class UnexpectedTokenException(Token token) : Exception($"Unexpected token: {token}.");
