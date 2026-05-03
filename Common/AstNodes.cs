using System.Text;

namespace Common;

public abstract record AstNode
{
    public sealed override string ToString() => this.GetTextForPrettyPrint();
}

public abstract record AstNodeWithLeftRight(AstNode LeftNode, AstNode RightNode) : AstNode;

#region Numbers
public record AddNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);
public record SubtractNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);
public record MultiplyNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);
public record DivideNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);
public record ModuloNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);

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

public record AndNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);

public record OrNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);

public record XorNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);
#endregion

#region Comparisons
public record EqualNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);

public record NotEqualNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);

public record GreaterThanNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);

public record GreaterThanOrEqualNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);

public record LessThanNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);

public record LessThanOrEqualNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);
#endregion

#region Text
public record TextNode(string Value) : AstNode;

public record ConcatNode(AstNode LeftNode, AstNode RightNode) : AstNodeWithLeftRight(LeftNode, RightNode);
#endregion

public static class AstNodeExtensions
{
    public static string GetTextForPrettyPrint(this AstNode node, bool[]? indentation = null)
    {
        return node switch
        {
            AstNodeWithLeftRight leftRightNode => leftRightNode.GetTextForPrettyPrint(indentation),
            BooleanNode booleanNode => booleanNode.Value ? "true" : "false",
            CreateVariableNode createVariableNode => createVariableNode.GetTextForPrettyPrint(indentation),
            GetVariableNode getVariableNode => $"{nameof(GetVariableNode)} \"{getVariableNode.Name}\"",
            NotNode notNode => notNode.GetTextForPrettyPrint(indentation),
            NumberNode numberNode => numberNode.Value.ToString(),
            SetVariableNode setVariableNode => setVariableNode.GetTextForPrettyPrint(indentation),
            TextNode textNode => $"\"{textNode.Value}\"",
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    }
    
    private static string GetTextForPrettyPrint(this NotNode node, bool[]? indentation = null)
    {
        indentation ??= [];
        var myIndentation = new List<bool>(indentation);
        
        StringBuilder builder = new();
        builder.Append(node.GetType().Name);
        
        builder.Append('\n');
        
        foreach (bool indent in indentation)
            builder.Append(indent ? "│   " : "    ");
        
        myIndentation.Add(false);

        builder.Append("└── ");
        builder.Append(node.Node.GetTextForPrettyPrint(myIndentation.ToArray()));

        return builder.ToString();
    }
    
    private static string GetTextForPrettyPrint(this CreateVariableNode node, bool[]? indentation = null)
    {
        indentation ??= [];
        var myIndentation = new List<bool>(indentation);
        
        StringBuilder builder = new();
        builder.Append(node.GetType().Name);
        builder.Append(" called \"");
        builder.Append(node.Name);
        builder.Append("\" of type ");
        builder.Append(node.Type);
        
        builder.Append('\n');
        
        foreach (bool indent in indentation)
            builder.Append(indent ? "│   " : "    ");
        
        myIndentation.Add(false);

        builder.Append("└── ");
        builder.Append(node.ValueNode.GetTextForPrettyPrint(myIndentation.ToArray()));

        return builder.ToString();
    }
    
    private static string GetTextForPrettyPrint(this SetVariableNode node, bool[]? indentation = null)
    {
        indentation ??= [];
        var myIndentation = new List<bool>(indentation);
        
        StringBuilder builder = new();
        builder.Append(node.GetType().Name);
        builder.Append(" \"");
        builder.Append(node.Name);
        builder.Append('\"');
        
        builder.Append('\n');
        
        foreach (bool indent in indentation)
            builder.Append(indent ? "│   " : "    ");
        
        myIndentation.Add(false);

        builder.Append("└── ");
        builder.Append(node.ValueNode.GetTextForPrettyPrint(myIndentation.ToArray()));

        return builder.ToString();
    }

    private static string GetTextForPrettyPrint(this AstNodeWithLeftRight node, bool[]? indentation = null)
    {
        indentation ??= [];
        var myIndentation = new List<bool>(indentation);
        
        StringBuilder builder = new();
        builder.Append(node.GetType().Name);
        
        builder.Append('\n');
        
        foreach (bool indent in indentation)
            builder.Append(indent ? "│   " : "    ");
        
        myIndentation.Add(true);

        builder.Append("├── ");
        builder.Append(node.LeftNode.GetTextForPrettyPrint(myIndentation.ToArray()));
        
        builder.Append('\n');
        
        foreach (bool indent in indentation)
            builder.Append(indent ? "│   " : "    ");
        
        myIndentation.RemoveAt(myIndentation.Count - 1);
        myIndentation.Add(false);
        
        builder.Append("└── ");
        builder.Append(node.RightNode.GetTextForPrettyPrint(myIndentation.ToArray()));
        
        return builder.ToString();
    }
}
