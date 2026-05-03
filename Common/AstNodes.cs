namespace XMLSharpCompiler;

public abstract record AstNode;

#region Numbers
public record AddNode(AstNode LeftNode, AstNode RightNode) : AstNode;
public record SubtractNode(AstNode LeftNode, AstNode RightNode) : AstNode;
public record MultiplyNode(AstNode LeftNode, AstNode RightNode) : AstNode;
public record DivideNode(AstNode LeftNode, AstNode RightNode) : AstNode;
public record ModuloNode(AstNode LeftNode, AstNode RightNode) : AstNode;

public record NumberNode(int Value) : AstNode;
#endregion

#region Variables
public record CreateVariableNode(string Name, XMLSType Type, AstNode ValueNode) : AstNode;

public record SetVariableNode(string Name, AstNode ValueNode) : AstNode;

public record GetVariableNode(string Name) : AstNode;
#endregion

#region Boolean
public record NotNode(AstNode Node) : AstNode;

public record BooleanNode(bool Value) : AstNode;

public record AndNode(AstNode LeftNode, AstNode RightNode) : AstNode;

public record OrNode(AstNode LeftNode, AstNode RightNode) : AstNode;

public record XorNode(AstNode LeftNode, AstNode RightNode) : AstNode;
#endregion

#region Comparisons
public record EqualNode(AstNode LeftNode, AstNode RightNode) : AstNode;

public record NotEqualNode(AstNode LeftNode, AstNode RightNode) : AstNode;

public record GreaterThanNode(AstNode LeftNode, AstNode RightNode) : AstNode;

public record GreaterThanOrEqualNode(AstNode LeftNode, AstNode RightNode) : AstNode;

public record LessThanNode(AstNode LeftNode, AstNode RightNode) : AstNode;

public record LessThanOrEqualNode(AstNode LeftNode, AstNode RightNode) : AstNode;
#endregion
