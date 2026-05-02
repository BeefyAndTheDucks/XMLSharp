namespace XMLSharpCompiler;

public interface ILexer
{
    Token[] Lex(string input);
}

public enum Token
{
    // add more
    VariableDefinition,
    Expression
}
