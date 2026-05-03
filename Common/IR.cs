namespace Common;

// IR stands for Intermediate Representation. We convert our AST nodes to an IR representation, which we then write to a file to let an interpreter/compiler use.
public class IR : IIR
{
    private int temporaryIndex;
    
    public IRInstruction[] FromAst(AstNode[] ast)
    {
        temporaryIndex = 0;
        
        List<IRInstruction> instructions = [];
        
        foreach (AstNode rootNode in ast)
        {
            instructions.AddRange(FromAst(rootNode));
        }

        return instructions.ToArray();
    }

    private IRInstruction[] FromAst(AstNode node)
    {
        List<IRInstruction> instructions = [];

        switch (node)
        {
            // Variables
            case CreateVariableNode createVariableNode:
                instructions.AddRange(FromAst(createVariableNode.ValueNode));
                instructions.Add(new IRInstruction(node.IrOperation, temporaryIndex++, 0, 0));
                break;
            case GetVariableNode:
                instructions.Add(new IRInstruction(node.IrOperation, 0, 0, temporaryIndex));
                break;
            case SetVariableNode setVariableNode:
                instructions.AddRange(FromAst(setVariableNode.ValueNode));
                instructions.Add(new IRInstruction(node.IrOperation, temporaryIndex++, 0, 0));
                break;
            
            // Datatypes
            case NumberNode numberNode:
                instructions.Add(new IRInstruction(node.IrOperation, 0, 0, temporaryIndex, numberNode.Value));
                break;
            case TextNode textNode:
                instructions.Add(new IRInstruction(node.IrOperation, 0, 0, temporaryIndex, textNode.Value));
                break;
            case BooleanNode booleanNode:
                instructions.Add(new IRInstruction(node.IrOperation, 0, 0, temporaryIndex, booleanNode.Value));
                break;
            
            // Arithmetics
            case AstNodeWithLeftRight lrNode:
                instructions.AddRange(FromAst(lrNode.LeftNode));
                temporaryIndex++;
                instructions.AddRange(FromAst(lrNode.RightNode));
                instructions.Add(new IRInstruction(lrNode.IrOperation, temporaryIndex - 1, temporaryIndex++, temporaryIndex));
                break;
            
            case AstNodeWithSingleChild singleChildNode:
                instructions.AddRange(FromAst(singleChildNode.Child));
                instructions.Add(new IRInstruction(singleChildNode.IrOperation, temporaryIndex++, 0, temporaryIndex));
                break;
        }
        
        return instructions.ToArray();
    }

    public void WriteToFile(string path, IRInstruction[] instructions)
    {
        throw new NotImplementedException();
    }

    public IRInstruction[] ReadFromFile(string path)
    {
        throw new NotImplementedException();
    }
}

public record IRInstruction(IROperation Operation, int Operand1, int Operand2, int Result, object? Data = null);

public enum IROperation
{
    Add,
    Sub,
    Mul,
    Div,
    Mod,
    
    GetVar,
    SetVar,
    CreateVar,
    
    Not,
    And,
    Or,
    Xor,
    
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    
    Concat,
    
    Constant,
    Print
}
