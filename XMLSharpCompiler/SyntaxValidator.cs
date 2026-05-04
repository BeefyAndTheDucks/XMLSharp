namespace XMLSharpCompiler;

public class SyntaxValidator
{
    public SyntaxError[] Validate(Token[] tokens)
    {
        List<SyntaxError> errors = [];

        foreach (ISyntaxRule rule in SyntaxRules.All)
        {
            if (rule is IBlockRule blockRule)
            {
                errors.AddRange(blockRule.Validate(tokens));
            }
        }

        List<Token[]> statements = SplitStatements(tokens);

        foreach (Token[] statement in statements)
        {
            errors.AddRange(ValidateStatement(statement));
        }

        return errors.ToArray();
    }

    private static List<Token[]> SplitStatements(Token[] tokens)
    {
        List<Token[]> statements = [];
        List<Token> current = [];

        foreach (Token token in tokens)
        {
            if (token is SemicolonToken)
            {
                current.Add(token);
                statements.Add(current.ToArray());
                current = [];
            }
            else if (token is EOFToken)
            {
                if (current.Count > 0)
                    statements.Add(current.ToArray());
                break;
            }
            else
            {
                current.Add(token);
            }
        }

        return statements;
    }

    private static IEnumerable<SyntaxError> ValidateStatement(Token[] statement)
    {
        HashSet<int> flagged = [];

        foreach (ISyntaxRule rule in SyntaxRules.All)
        {
            if (rule is IStatementRule statementRule)
            {
                SyntaxError? error = statementRule.Validate(statement);
                if (error is not null)
                    yield return error;
            }
            if (rule is ITokenRule tokenRule)
            {
                for (int i = 0; i < statement.Length; i++)
                {
                    if (flagged.Contains(i)) continue;

                    SyntaxError? error = tokenRule.Validate(statement, i);
                    if (error is not null) {
                        flagged.Add(i);
                        yield return error;
                    }
                }
            }
        }
    }
}