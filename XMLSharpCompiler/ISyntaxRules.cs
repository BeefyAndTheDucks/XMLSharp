using Common;

namespace XMLSharpCompiler;

public interface ISyntaxRule;

public interface ITokenRule : ISyntaxRule
{
    Diagnostic? Validate(Token[] statement, int index);
}

public interface IStatementRule : ISyntaxRule
{
    Diagnostic? Validate(Token[] statement);
}

public interface IBlockRule : ISyntaxRule
{
    Diagnostic[] Validate(Token[] tokens);
}