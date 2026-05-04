using System.Text;

namespace Common;

public abstract record AstNode
{
    public sealed override string ToString() => this.GetTextForPrettyPrint();
}

public abstract record AstNodeWithLeftRight(AstNode LeftNode, AstNode RightNode, IROperation IrOperation) : AstNode;

public abstract record AstNodeWithSingleChild(AstNode Child, IROperation IrOperation) : AstNode;

public record BlockNode(AstNode[] Nodes) : AstNode;

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
public record CreateVariableNode(string Name, XMLSType Type, AstNode ValueNode) : AstNode;

public record SetVariableNode(string Name, AstNode ValueNode) : AstNode;

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
#endregion

public static class AstNodeExtensions
{
    public static string GetTextForPrettyPrint(this AstNode node, bool[]? indentation = null)
    {
        return node switch
        {
            BlockNode block => block.GetTextForPrettyPrint(indentation),
            AstNodeWithLeftRight leftRightNode => leftRightNode.GetTextForPrettyPrint(indentation),
            AstNodeWithSingleChild singleChildNode => singleChildNode.GetTextForPrettyPrint(indentation),
            BooleanNode booleanNode => booleanNode.Value ? "true" : "false",
            CreateVariableNode createVariableNode => createVariableNode.GetTextForPrettyPrint(indentation),
            GetVariableNode getVariableNode => $"{nameof(GetVariableNode)} \"{getVariableNode.Name}\"",
            NumberNode numberNode => numberNode.Value.ToString(),
            SetVariableNode setVariableNode => setVariableNode.GetTextForPrettyPrint(indentation),
            TextNode textNode => $"\"{textNode.Value}\"",
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    }
    
    private static string GetTextForPrettyPrint(this BlockNode node, bool[]? indentation = null)
    {
        indentation ??= [];
        var myIndentation = new List<bool>(indentation);
        
        StringBuilder builder = new();
        builder.Append(node.GetType().Name);

        for (int i = 0; i < node.Nodes.Length; i++)
        {
            bool last = i >= node.Nodes.Length - 1;
            bool first = i == 0;
            AstNode currentNode = node.Nodes[i];
            
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
    
    private static string GetTextForPrettyPrint(this AstNodeWithSingleChild node, bool[]? indentation = null)
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
        builder.Append(node.Child.GetTextForPrettyPrint(myIndentation.ToArray()));

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
