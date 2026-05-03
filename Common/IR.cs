using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common;

// IR stands for Intermediate Representation. We convert our AST nodes to an IR representation, which we then write to a file to let an interpreter/compiler use.
public class IR : IIR
{
    private int _temporaryIndex;

    private readonly JsonSerializerOptions _options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    };
    
    public IRInstruction[] FromAst(AstNode[] ast)
    {
        _temporaryIndex = 0;
        
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
                instructions.Add(new IRInstruction(node.IrOperation, _temporaryIndex++, 0, 0));
                break;
            case GetVariableNode:
                instructions.Add(new IRInstruction(node.IrOperation, 0, 0, _temporaryIndex));
                break;
            case SetVariableNode setVariableNode:
                instructions.AddRange(FromAst(setVariableNode.ValueNode));
                instructions.Add(new IRInstruction(node.IrOperation, _temporaryIndex++, 0, 0));
                break;
            
            // Datatypes
            case NumberNode numberNode:
                instructions.Add(new IRInstruction(node.IrOperation, 0, 0, _temporaryIndex, numberNode.Value));
                break;
            case TextNode textNode:
                instructions.Add(new IRInstruction(node.IrOperation, 0, 0, _temporaryIndex, textNode.Value));
                break;
            case BooleanNode booleanNode:
                instructions.Add(new IRInstruction(node.IrOperation, 0, 0, _temporaryIndex, booleanNode.Value));
                break;
            
            // Arithmetics
            case AstNodeWithLeftRight lrNode:
                instructions.AddRange(FromAst(lrNode.LeftNode));
                _temporaryIndex++;
                instructions.AddRange(FromAst(lrNode.RightNode));
                instructions.Add(new IRInstruction(lrNode.IrOperation, _temporaryIndex - 1, _temporaryIndex++, _temporaryIndex));
                break;
            
            case AstNodeWithSingleChild singleChildNode:
                instructions.AddRange(FromAst(singleChildNode.Child));
                instructions.Add(new IRInstruction(singleChildNode.IrOperation, _temporaryIndex++, 0, _temporaryIndex));
                break;
        }
        
        return instructions.ToArray();
    }

    public void WriteToFile(FileInfo file, IRInstruction[] instructions)
    {
        string json = JsonSerializer.Serialize(instructions, _options);
        File.WriteAllText(file.FullName, json);
    }

    public IRInstruction[]? ReadFromFile(FileInfo file)
    {
        return JsonSerializer.Deserialize<IRInstruction[]>(File.ReadAllText(file.FullName), _options);
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
