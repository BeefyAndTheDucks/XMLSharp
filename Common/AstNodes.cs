using System.Globalization;
using System.Text;

namespace Common;

public abstract record AstNode
{
    public sealed override string ToString() => this.GetTextForPrettyPrint();
}

public abstract record AstNodeWithChildren(AstNode[] Children, string? PrettyPrintLabel = null) : AstNode
{
    public virtual bool Equals(AstNodeWithChildren? other)
    {
        if (other is null || GetType() != other.GetType()) return false;
        
        return PrettyPrintLabel == other.PrettyPrintLabel &&
               Children.SequenceEqual(other.Children); 
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(PrettyPrintLabel);
        foreach (AstNode child in Children) hash.Add(child);
        return hash.ToHashCode();
    }
}

public abstract record AstNodeWithLeftRight(AstNode LeftNode, AstNode RightNode, IROperation IrOperation, string? PrettyPrintLabel = null) : AstNodeWithChildren([LeftNode, RightNode], PrettyPrintLabel);

public abstract record AstNodeWithSingleChild(AstNode Child, IROperation IrOperation, string? PrettyPrintLabel = null) : AstNodeWithChildren([Child], PrettyPrintLabel);

public record BlockNode(AstNode[] Nodes) : AstNodeWithChildren(Nodes);

#region Numbers
public record AddNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.Add);
public record SubtractNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.Sub);
public record MultiplyNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.Mul);
public record DivideNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.Div);
public record ModuloNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.Mod);

public record NumberNode(int Value) : AstNode;
public record DecimalNode(float Value) : AstNode;

#endregion

#region Variables
public record CreateVariableNode(string Name, XMLSType Type, AstNode ValueNode) : AstNodeWithChildren([ValueNode], $"called \"{Name}\" of type {Type}");

public record SetVariableNode(string Name, AstNode ValueNode) : AstNodeWithChildren([ValueNode], Name);

public record GetVariableNode(string Name) : AstNode;
#endregion

#region Boolean
public record NotNode(AstNode Node) : AstNodeWithSingleChild(Node, IROperation.Not);

public record BooleanNode(bool Value) : AstNode;

public record AndNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.And);

public record OrNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.Or);

public record XorNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.Xor);
#endregion

#region Comparisons
public record EqualNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.Equal);

public record NotEqualNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.NotEqual);

public record GreaterThanNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.GreaterThan);

public record GreaterThanOrEqualNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.GreaterThanOrEqual);

public record LessThanNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.LessThan);

public record LessThanOrEqualNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.LessThanOrEqual);
#endregion

#region Text
public record TextNode(string Value) : AstNode;

public record ConcatNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode, IROperation.Concat);
#endregion

#region Functions
public record PrintNode(AstNode Value) : AstNodeWithSingleChild(Value, IROperation.Print);

public record FunctionNode(string Name, AstNode Contents) : AstNodeWithChildren([Contents], Name);
public record ReturnNode(AstNode Value) : AstNodeWithChildren([Value]);
public record CallFunctionNode(string Name, AstNode[] Arguments) : AstNodeWithChildren(Arguments, Name);

public record SetParameterNode(string Name, AstNode Value) : AstNodeWithChildren([Value], Name);
public record GetParameterNode(string Name) : AstNode;
#endregion

#region Control Flow
public record IfNode(AstNode Condition, AstNode IfTrue, AstNode? IfFalse) : AstNodeWithChildren([Condition, IfTrue, IfFalse ?? new TextNode("No IfFalse.")]);

public record WhileNode(AstNode Condition, AstNode Loop) : AstNodeWithChildren([Condition, Loop]);
#endregion

public static class AstNodeExtensions
{
    extension(AstNode node)
    {
        public void PrettyPrint()
        {
            Console.WriteLine(node.GetTextForPrettyPrint());
        }

        public string GetTextForPrettyPrint(bool[]? indentation = null)
        {
            return node switch
            {
                AstNodeWithChildren block => block.GetTextForPrettyPrint(indentation),
                GetVariableNode getVariableNode => $"{nameof(GetVariableNode)} \"{getVariableNode.Name}\"",
                BooleanNode booleanNode => booleanNode.Value ? "true" : "false",
                NumberNode numberNode => numberNode.Value.ToString(),
                DecimalNode decimalNode => decimalNode.Value.ToString(CultureInfo.InvariantCulture),
                TextNode textNode => $"\"{textNode.Value}\"",
                GetParameterNode getParameterNode => $"{nameof(GetParameterNode)} \"{getParameterNode.Name}\"",
                _ => throw new NotSupportedException($"Pretty print for {node.GetType().Name} not implemented yet.")
            };
        }
    }

    private static string GetTextForPrettyPrint(this AstNodeWithChildren node, bool[]? indentation = null)
    {
        indentation ??= [];
        var myIndentation = new List<bool>(indentation);
        
        StringBuilder builder = new();
        builder.Append(node.GetType().Name);
        builder.Append(' ');
        builder.Append(node.PrettyPrintLabel);

        for (int i = 0; i < node.Children.Length; i++)
        {
            bool last = i >= node.Children.Length - 1;
            bool first = i == 0;
            AstNode currentNode = node.Children[i];
            
            builder.Append('\n');
        
            foreach (bool indent in indentation)
                builder.Append(indent ? "│   " : "    ");
        
            if (!first)
                myIndentation.RemoveAt(myIndentation.Count - 1);
            myIndentation.Add(!last);

            builder.Append(last ? "└── " : "├── ");
            builder.Append(currentNode.GetTextForPrettyPrint(myIndentation.ToArray()));
        }
        
        return builder.ToString();
    }
}
