using Common;

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
                return new CreateVariableNode(identifierToken.Name, variableDefinitionToken.Type, ParseExpression(tokens.Skip(currentIndex).ToArray()));
            }
            case IdentifierToken identifierToken:
            {
                currentIndex++;
                ConvertOrThrow<AssignmentToken>(tokens[currentIndex++]);
                return new SetVariableNode(identifierToken.Name, ParseExpression(tokens.Skip(currentIndex).ToArray()));
            }
        }
        
        throw new UnexpectedTokenException(currentToken);
    }

    internal static AstNode ParseExpression(Token[] tokens, int? stopIndex = null)
    {
        int leastPrecedence = GetLeastPrecedenceNodeIndex(tokens, stopIndex);
        
        return leastPrecedence == -1
            ? FirstPossibleNode(tokens)
            : ParseOperation(tokens, leastPrecedence);
    }

    private static AstNode FirstPossibleNode(Token[] tokens)
    {
        foreach (Token token in tokens)
        {
            try
            {
                return NodeFromToken(token);
            } catch (InvalidOperationException) {}
        }
        throw new InvalidOperationException("Cannot create node from tokens.");
    }

    private static AstNode NodeFromToken(Token token)
    {
        return token switch
        {
            NumberToken number => new NumberNode(number.Value),
            IdentifierToken identifier => new GetVariableNode(identifier.Name),
            TextToken text => new TextNode(text.Text),
            YesToken => new BooleanNode(true),
            NoToken => new BooleanNode(false),
            
            _ => throw new InvalidOperationException("Cannot create node from token: " + token + ".")
        };
    }

    private static int GetLeastPrecedenceNodeIndex(Token[] tokens, int? stopIndex = null)
    {
        int leastPrecedence = int.MaxValue;
        int leastPrecedenceIndex = -1;
        int precedenceBonus = 0;
        for (int i = 0; i < tokens.Length; i++)
        {
            if (i >= stopIndex)
                break;

            if (tokens[i] is SemicolonToken)
                break;
            
            Token token = tokens[i];

            switch (token)
            {
                case OpenParenToken:
                    precedenceBonus += 100;
                    continue;
                case CloseParenToken:
                    precedenceBonus -= 100;
                    continue;
            }

            int tokenPrecedence = GetPrecedence(token);
            int combinedPrecedence = tokenPrecedence + precedenceBonus;
            
            if (combinedPrecedence > leastPrecedence || tokenPrecedence < 0) continue;
            leastPrecedence = combinedPrecedence;
            leastPrecedenceIndex = i;
        }
        
        return leastPrecedenceIndex;
    }

    private static int GetPrecedence(Token token)
    {
        return token switch
        {
            // Numbers
            AddToken => 10,
            SubtractToken => 10,
            MultiplyToken => 20,
            DivideToken => 20,
            ModuloToken => 20, // according to https://www.calc-tools.com/formulas/order-of-operations-understanding-modulo, modulo has the same OOO as Mult and Divide.
            
            // Boolean
            AndToken => 10,
            OrToken => 10,
            XorToken => 10,
            NotToken => 20,
            
            // Text
            ConcatToken => 10,
            
            // Comparisons
            EqualsToken => 1,
            NotEqualsToken => 1,
            GreaterToken => 1,
            GreaterOrEqualsToken => 1,
            LessToken => 1,
            LessOrEqualsToken => 1,
            
            _ => -1,
        };
    }
    
    private static AstNode ParseOperation(Token[] tokens, int currentIndex)
    {
        return tokens[currentIndex] switch
        {
            // Numbers
            AddToken => new AddNode(GetLhs(), GetRhs()),
            SubtractToken => new SubtractNode(GetLhs(), GetRhs()),
            MultiplyToken => new MultiplyNode(GetLhs(), GetRhs()),
            DivideToken => new DivideNode(GetLhs(), GetRhs()),
            ModuloToken => new ModuloNode(GetLhs(), GetRhs()),
            
            // Comparisons
            GreaterToken => new GreaterThanNode(GetLhs(), GetRhs()),
            GreaterOrEqualsToken => new GreaterThanOrEqualNode(GetLhs(), GetRhs()),
            LessToken => new LessThanNode(GetLhs(), GetRhs()),
            LessOrEqualsToken => new LessThanOrEqualNode(GetLhs(), GetRhs()),
            EqualsToken => new EqualNode(GetLhs(), GetRhs()),
            NotEqualsToken => new NotEqualNode(GetLhs(), GetRhs()),
            
            // Boolean
            AndToken => new AndNode(GetLhs(), GetRhs()),
            OrToken => new OrNode(GetLhs(), GetRhs()),
            XorToken => new XorNode(GetLhs(), GetRhs()),
            NotToken => new NotNode(GetRhs()),
            
            // Text
            ConcatToken => new ConcatNode(GetLhs(), GetRhs()),
            
            _ => GetLhs()
        };

        AstNode GetLhs() => ParseExpression(tokens.Take(currentIndex).ToArray());

        AstNode GetRhs() => ParseExpression(tokens.Skip(currentIndex + 1).ToArray());
    }

    private static T ConvertOrThrow<T>(Token token)
    {
        if (token is T t) return t;
        throw new UnexpectedTokenException(token);
    }
}

public class UnexpectedTokenException(Token token) : Exception($"Unexpected token: {token}.");
