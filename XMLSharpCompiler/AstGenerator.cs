using Common;

namespace XMLSharpCompiler;

public class AstGenerator : IAstGenerator
{
    private bool _dirty;
    private readonly List<string> _arguments = [];
    
    public AstNode Generate(Token[] tokens)
    {
        if (_dirty)
            throw new InvalidOperationException($"Cannot reuse AstGenerator instances after {nameof(Generate)} has been called.");
        _dirty = true;
        
        int index = 0;
        List<AstNode> nodes = [];

        while (tokens[index] is not EOFToken)
        {
            nodes.Add(Parse(tokens, out index, index));
            
            index++;
        }

        if (nodes.Count == 1)
            return nodes[0];
        return new BlockNode(nodes.ToArray());
    }
    
    private AstNode ParseBlock(Token[] tokens)
    {
        int index = 0;
        List<AstNode> nodes = [];
        
        int blockEnd = FindBlockEndIndex(tokens, 0, 1);

        while (index < blockEnd)
        {
            nodes.Add(Parse(tokens, out index, index));

            index++;
        }
        
        if (nodes.Count == 1)
            return nodes[0];
        return new BlockNode(nodes.ToArray());
    }
    
    private AstNode Parse(Token[] tokens, out int endIndex, int currentIndex)
    {
        Token currentToken = tokens[currentIndex];

        switch (currentToken)
        {
            case TypeToken typeToken:
            {
                currentIndex++;
                IdentifierToken identifierToken = ConvertOrThrow<IdentifierToken>(tokens[currentIndex++]);
                ConvertOrThrow<AssignmentToken>(tokens[currentIndex++]);
                endIndex = FindNextTokenOfType<SemicolonToken>(tokens, currentIndex);
                return new CreateVariableNode(identifierToken.Name, typeToken.Type, ParseExpression(tokens.Skip(currentIndex).ToArray()));
            }
            case IdentifierToken identifierToken:
            {
                if (tokens[currentIndex + 1] is OpenParenToken)
                {
                    endIndex = FindNextTokenOfType<SemicolonToken>(tokens, currentIndex);
                    return ParseFunctionCall(tokens, currentIndex, identifierToken);
                }
                
                currentIndex++;
                ConvertOrThrow<AssignmentToken>(tokens[currentIndex++]);
                endIndex = FindNextTokenOfType<SemicolonToken>(tokens, currentIndex);
                AstNode expression = ParseExpression(tokens.Skip(currentIndex).ToArray());
                if (_arguments.Contains(identifierToken.Name))
                    return new SetParameterNode(identifierToken.Name, expression);
                return new SetVariableNode(identifierToken.Name, expression);
            }
            
            // Functions
            case PrintToken:
            {
                currentIndex++;
                endIndex = FindNextTokenOfType<SemicolonToken>(tokens, currentIndex);
                return new PrintNode(ParseExpression(tokens.Skip(currentIndex).ToArray()));
            }

            case FunctionToken:
            {
                currentIndex++;
                XMLSType? returnType = null;
                if (tokens[currentIndex] is TypeToken type)
                {
                    returnType = type.Type;
                    currentIndex++;
                }
                IdentifierToken identifierToken = ConvertOrThrow<IdentifierToken>(tokens[currentIndex++]);
                ConvertOrThrow<OpenParenToken>(tokens[currentIndex++]);
                List<(XMLSType Type, string ParamName)> parameters = [];
                while (tokens[currentIndex] is not CloseParenToken)
                {
                    parameters.Add((ConvertOrThrow<TypeToken>(tokens[currentIndex++]).Type,
                        ConvertOrThrow<IdentifierToken>(tokens[currentIndex++]).Name));
                    if (tokens[currentIndex] is SeparatorToken)
                        currentIndex++;
                }
                _arguments.AddRange(parameters.Select(p => p.ParamName));
                currentIndex++;
                int blockEnd = FindBlockEndIndex(tokens, currentIndex++);
                AstNode block = ParseBlock(tokens.Skip(currentIndex).Take(blockEnd - currentIndex + 1).ToArray());
                if (block is BlockNode blockNode)
                {
                    if (blockNode.Nodes[^1] is not ReturnNode)
                        block = new BlockNode(blockNode.Nodes.Append(new VoidReturnNode()).ToArray());
                }
                else
                {
                    if (block is not ReturnNode)
                        block = new BlockNode([block, new VoidReturnNode()]);
                }
                
                int returnIndex = FindNextTokenOfType<ReturnToken>(tokens, currentIndex);
                bool hasReturn = returnIndex > -1 && returnIndex < blockEnd;
                if (returnType != null && !hasReturn)
                    throw new InvalidOperationException("Function must return a value if it has a return type."); // TODO: Convert to new error/diagnostic system
                foreach (string parameter in parameters.Select(p => p.ParamName))
                    _arguments.Remove(parameter);
                endIndex = blockEnd;
                return new FunctionNode(identifierToken.Name, returnType, block, parameters.Select(p => p.ParamName).ToArray());
            }
            
            case ReturnToken:
            {
                currentIndex++;
                if (tokens[currentIndex] is SemicolonToken)
                {
                    endIndex = currentIndex;
                    return new VoidReturnNode();
                }
                endIndex = FindNextTokenOfType<SemicolonToken>(tokens, currentIndex);
                return new ReturnNode(ParseExpression(tokens.Skip(currentIndex).ToArray()));
            }
            
            // Control-flow
            case IfToken:
            {
                int blockBeginning = FindNextTokenOfType<BeginBlockToken>(tokens, currentIndex);
                AstNode condition = ParseExpression(tokens.Skip(currentIndex + 1).Take(blockBeginning - currentIndex - 1).ToArray());
                
                int blockEnd = FindBlockEndIndex(tokens, blockBeginning - 1);
                AstNode block = ParseBlock(tokens.Skip(blockBeginning + 1).Take(blockEnd - blockBeginning).ToArray());
                
                if (tokens[blockEnd + 1] is not ElseToken)
                {
                    endIndex = blockEnd;
                    return new IfNode(condition, block, null);
                }
                
                int elseBlockBeginning = FindNextTokenOfType<BeginBlockToken>(tokens, blockEnd + 1);
                int elseBlockEnd = FindBlockEndIndex(tokens, elseBlockBeginning - 1);
                AstNode elseBlock = ParseBlock(tokens.Skip(elseBlockBeginning + 1).Take(elseBlockEnd - elseBlockBeginning).ToArray());

                endIndex = elseBlockEnd;
                
                return new IfNode(condition, block, elseBlock);
            }

            case WhileToken:
            {
                int blockBeginning = FindNextTokenOfType<BeginBlockToken>(tokens, currentIndex);
                AstNode condition = ParseExpression(tokens.Skip(currentIndex + 1).Take(blockBeginning - currentIndex - 1).ToArray());
                
                int blockEnd = FindBlockEndIndex(tokens, blockBeginning - 1);
                AstNode block = ParseBlock(tokens.Skip(blockBeginning + 1).Take(blockEnd - blockBeginning).ToArray());

                endIndex = blockEnd;
                
                return new WhileNode(condition, block);
            }

            case ForToken:
            {
                currentIndex++;
                ConvertOrThrow<OpenParenToken>(tokens[currentIndex++]);
                int loopHeaderEnd = FindNextTokenOfType<SemicolonToken>(tokens, currentIndex);
                AstNode loopHeader = Parse(tokens.Skip(currentIndex).Take(loopHeaderEnd - currentIndex + 1).ToArray(), out _, 0);
                int conditionEnd = FindNextTokenOfType<SemicolonToken>(tokens, loopHeaderEnd + 1);
                AstNode condition = ParseExpression(tokens.Skip(loopHeaderEnd + 1).Take(conditionEnd - loopHeaderEnd).ToArray());
                int loopFooterEnd = FindNextTokenOfType<CloseParenToken>(tokens, conditionEnd + 1);
                AstNode loopFooter = Parse(tokens.Skip(conditionEnd + 1).Take(loopFooterEnd - conditionEnd - 1).Append(new SemicolonToken()).ToArray(), out _, 0);
                
                int loopBlockEnd = FindBlockEndIndex(tokens, loopFooterEnd);
                AstNode block = ParseBlock(tokens.Skip(loopFooterEnd + 2).Take(loopBlockEnd - loopFooterEnd).ToArray());

                endIndex = loopBlockEnd;
                return new BlockNode([
                    loopHeader,
                    new WhileNode(condition, new BlockNode([
                        block,
                        loopFooter
                    ]))
                ]);
            }
        }
        
        throw new UnexpectedTokenException(currentToken);
    }

    internal AstNode ParseExpression(Token[] tokens, int? stopIndex = null)
    {
        int leastPrecedence = GetLeastPrecedenceNodeIndex(tokens, stopIndex);
        
        return leastPrecedence == -1
            ? FirstPossibleNode(tokens)
            : ParseOperation(tokens, leastPrecedence);
    }

    internal static int FindNextTokenOfType<T>(Token[] tokens, int startIndex) where T : Token
    {
        int index = startIndex;
        while (index < tokens.Length && tokens[index] is not EOFToken)
        {
            if (tokens[index] is T)
                return index;
            index++;
        }
        return -1;
    }

    internal static int FindNextTokenInSameParenDepthOfType<T>(Token[] tokens, int startIndex) where T : Token
    {
        int index = startIndex;
        int parensDepth = 0;
        while (index < tokens.Length && tokens[index] is not EOFToken)
        {
            switch (tokens[index])
            {
                case T:
                    if (parensDepth == 0)
                        return index;
                    if (typeof(T) == typeof(OpenParenToken))
                        parensDepth++;
                    if (typeof(T) == typeof(CloseParenToken))
                        parensDepth--;
                    break;
                case OpenParenToken:
                    parensDepth++;
                    break;
                case CloseParenToken:
                    parensDepth--;
                    break;
            }
            
            index++;
        }
        return -1;
    }
    
    internal static int FindBlockEndIndex(Token[] tokens, int startIndex, int startDepth = 0)
    {
        int index = startIndex;
        int blockDepth = startDepth;
        while (index < tokens.Length && tokens[index] is not EOFToken)
        {
            if (tokens[index] is BeginBlockToken)
                blockDepth++;
            if (tokens[index] is EndBlockToken)
            {
                blockDepth--;
                switch (blockDepth)
                {
                    case 0:
                        return index;
                    case < 0:
                        throw new InvalidOperationException("Block depth cannot be negative.");
                }
            }
            index++;
        }
        return -1;
    }

    private AstNode FirstPossibleNode(Token[] tokens)
    {
        for (int i = 0; i < tokens.Length; i++)
        {
            try
            {
                return NodeFromTokens(tokens, i);
            } catch (InvalidOperationException) {}
        }
        throw new InvalidOperationException($"Cannot create node from tokens.\n{tokens.GetTextForPrettyPrint()}");
    }

    private AstNode NodeFromTokens(Token[] tokens, int index)
    {
        Token token = tokens[index];
        return token switch
        {
            NumberToken number => new NumberNode(number.Value),
            DecimalToken dec => new DecimalNode(dec.Value),
            IdentifierToken => ParseIdentifierInExpression(tokens, index),
            TextToken text => new TextNode(text.Text),
            YesToken => new BooleanNode(true),
            NoToken => new BooleanNode(false),
            
            _ => throw new InvalidOperationException("Cannot create node from token: " + token + ".")
        };
    }

    private AstNode ParseIdentifierInExpression(Token[] tokens, int currentIndex)
    {
        IdentifierToken identifierToken = ConvertOrThrow<IdentifierToken>(tokens[currentIndex]); // This should never throw.
        Token? nextToken = null;
        try
        {
            nextToken = tokens[currentIndex + 1];
        } catch (IndexOutOfRangeException) {}
        if (nextToken is not OpenParenToken)
        {
            if (_arguments.Contains(identifierToken.Name))
                return new GetParameterNode(identifierToken.Name);
            return new GetVariableNode(ConvertOrThrow<IdentifierToken>(tokens[currentIndex]).Name);
        }
        
        // Function call...
        return ParseFunctionCall(tokens, currentIndex, identifierToken);
    }

    private CallFunctionNode ParseFunctionCall(Token[] tokens, int currentIndex, IdentifierToken identifierToken)
    {
        currentIndex += 2; // skip over identifier and open paren.
        List<AstNode> arguments = [];
        while (tokens[currentIndex] is not CloseParenToken)
        {
            int endIndex = FindNextTokenInSameParenDepthOfType<SeparatorToken>(tokens, currentIndex);
            bool foundSeparator = endIndex != -1;
            if (!foundSeparator)
                endIndex = FindNextTokenInSameParenDepthOfType<CloseParenToken>(tokens, currentIndex);
            if (endIndex == -1)
                throw new InvalidOperationException("Cannot find closing paren for function call.");
            arguments.Add(ParseExpression(tokens.Skip(currentIndex).Take(endIndex - currentIndex).ToArray()));
            currentIndex = foundSeparator ? endIndex + 1 : endIndex;
        }

        return new CallFunctionNode(identifierToken.Name, arguments.ToArray());
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
    
    private AstNode ParseOperation(Token[] tokens, int currentIndex)
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
            
            _ => throw new InvalidOperationException("Cannot parse operation: " + tokens[currentIndex] + ".")
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
