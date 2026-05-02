namespace XMLSharpCompiler;

public interface IAstGenerator
{
    AstNode[] Generate(Token[] tokens);
}

