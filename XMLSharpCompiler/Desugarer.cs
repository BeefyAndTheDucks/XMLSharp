namespace XMLSharpCompiler;

public class Desugarer : IDesugarer
{
    // Remove syntactic sugar such as elif -> else { if() { } }
    public Token[] Desugar(Token[] tokens)
    {
        List<int> elifDepths = [];
        List<Token> desugaredTokens = [];
        int currentDepth = 0;
        
        foreach (Token token in tokens)
        {
            switch (token)
            {
                case BeginBlockToken:
                    currentDepth++;
                    desugaredTokens.Add(token);
                    break;
                case EndBlockToken:
                    currentDepth--;
                    desugaredTokens.Add(token);
                    if (elifDepths.Contains(currentDepth))
                    {
                        desugaredTokens.Add(new EndBlockToken());
                        currentDepth--;
                        elifDepths.Remove(currentDepth);
                    }
                    break;
                case ElifToken:
                    desugaredTokens.Add(new ElseToken());
                    desugaredTokens.Add(new BeginBlockToken());
                    
                    currentDepth++;
                    elifDepths.Add(currentDepth);
                    
                    desugaredTokens.Add(new IfToken());
                    break;
                default:
                    desugaredTokens.Add(token);
                    break;
            }
        }
        
        return desugaredTokens.ToArray();
    }
}