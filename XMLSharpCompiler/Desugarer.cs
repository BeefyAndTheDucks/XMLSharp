using Common;

namespace XMLSharpCompiler;

// Remove syntactic sugar such as elif -> else { if() { } }
public class Desugarer : ITokenProcessor
{

    private static (Token[] TokensOut, bool Changed) DesugarPass(Token[] tokens)
    {
        List<int> elifDepths = [];
        List<Token> desugaredTokens = [];
        int currentDepth = 0;
        
        bool changed = false;
        
        for (int i = 0; i < tokens.Length; i++)
        {
            Token token = tokens[i];
            switch (token)
            {
                case BeginBlockToken:
                    currentDepth++;
                    desugaredTokens.Add(token);
                    break;
                case EndBlockToken:
                    currentDepth--;
                    desugaredTokens.Add(token);
                    Token nextToken = tokens[i + 1];
                    if (elifDepths.Contains(currentDepth) && nextToken is not ElseToken)
                    {
                        changed = true;
                        desugaredTokens.Add(new EndBlockToken());
                        currentDepth--;
                        elifDepths.Remove(currentDepth);
                    }
                    break;
                case ElifToken:
                    changed = true;
                    desugaredTokens.Add(new ElseToken());
                    desugaredTokens.Add(new BeginBlockToken());
                    
                    currentDepth++;
                    elifDepths.Add(currentDepth);
                    
                    desugaredTokens.Add(new IfToken());
                    break;
                case IncrementToken:
                    changed = true;
                    desugaredTokens.Add(new IncrementByToken());
                    desugaredTokens.Add(new NumberToken(1));
                    break;
                case DecrementToken:
                    changed = true;
                    desugaredTokens.Add(new DecrementByToken());
                    desugaredTokens.Add(new NumberToken(1));
                    break;
                case IncrementByToken:
                    changed = true;
                    desugaredTokens.Add(new AssignmentToken());
                    desugaredTokens.Add(tokens[i - 1]);
                    desugaredTokens.Add(new AddToken());
                    break;
                case DecrementByToken:
                    changed = true;
                    desugaredTokens.Add(new AssignmentToken());
                    desugaredTokens.Add(tokens[i - 1]);
                    desugaredTokens.Add(new SubtractToken());
                    break;
                case MultiplyByToken:
                    changed = true;
                    desugaredTokens.Add(new AssignmentToken());
                    desugaredTokens.Add(tokens[i - 1]);
                    desugaredTokens.Add(new MultiplyToken());
                    break;
                case DivideByToken:
                    changed = true;
                    desugaredTokens.Add(new AssignmentToken());
                    desugaredTokens.Add(tokens[i - 1]);
                    desugaredTokens.Add(new DivideToken());
                    break;
                case ModuloByToken:
                    changed = true;
                    desugaredTokens.Add(new AssignmentToken());
                    desugaredTokens.Add(tokens[i - 1]);
                    desugaredTokens.Add(new ModuloToken());
                    break;
                default:
                    desugaredTokens.Add(token);
                    break;
            }
        }
        
        return (desugaredTokens.ToArray(), changed);
    }

    public (Token[], Diagnostic[]) Process(Token[] tokens, Diagnostic[] diagnostics)
    {
        Token[] desugaredTokens = [..tokens];
        bool changed;
        do
        {
            (desugaredTokens, changed) = DesugarPass(desugaredTokens);
        } while (changed);
        
        return (desugaredTokens.ToArray(), diagnostics);
    }
}