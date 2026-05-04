namespace XMLSharpCompiler;

public interface ISyntaxRule;

public interface ITokenRule : ISyntaxRule
{
    SyntaxError? Validate(Token[] statement, int index);
}

public interface IStatementRule : ISyntaxRule
{
    SyntaxError? Validate(Token[] statement);
}

public interface IBlockRule : ISyntaxRule
{
    SyntaxError[] Validate(Token[] tokens);
}