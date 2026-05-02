namespace XMLSharpCompiler;

public interface IAstGenerator
{
    AstNode Generate(Token[] tokens);
}

public abstract record AstNode;

public record CreateVariableNode(string Name, XMLSType Type, AstNode ValueNode) : AstNode;

public record AddNode(AstNode LeftNode, AstNode RightNode) : AstNode;

public record NumberNode(int Value) : AstNode;

public record GetVariableNode(string Name) : AstNode;

public record SetVariableNode(string Name, AstNode ValueNode) : AstNode;
