namespace XMLSharpCompiler;

public interface IDesugarer
{
    Token[] Desugar(Token[] tokens);
}