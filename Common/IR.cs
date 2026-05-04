using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common;

// IR stands for Intermediate Representation. We convert our AST nodes to an IR representation, which we then write to a file to let an interpreter/compiler use.
public class IR : IIR
{
    private int _temporaryIndex;
    private int _variableIndex;
    
    private Dictionary<string, int> _variableNameToVariableIndexTable = new();

    private readonly JsonSerializerOptions _options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    };
    
    public IRInstruction[] FromAst(AstNode[] ast)
    {
        _variableNameToVariableIndexTable.Clear();
        
        _temporaryIndex = 0;
        _variableIndex = 0;
        
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
                _variableNameToVariableIndexTable.Add(createVariableNode.Name, _variableIndex++);
                instructions.Add(new IRInstruction(node.IrOperation, _variableNameToVariableIndexTable[createVariableNode.Name], _temporaryIndex++, 0));
                break;
            case GetVariableNode getVariableNode:
                instructions.Add(new IRInstruction(node.IrOperation, _variableNameToVariableIndexTable[getVariableNode.Name], 0, _temporaryIndex));
                break;
            case SetVariableNode setVariableNode:
                instructions.AddRange(FromAst(setVariableNode.ValueNode));
                instructions.Add(new IRInstruction(node.IrOperation, _variableNameToVariableIndexTable[setVariableNode.Name], _temporaryIndex++, 0));
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

public record IRInstruction(
    [property: JsonPropertyName("O")] IROperation Operation,
    [property: JsonPropertyName("1")] int Operand1,
    [property: JsonPropertyName("2")] int Operand2,
    [property: JsonPropertyName("R")] int Result,
    [property: JsonPropertyName("D")] object? Data = null
);

public enum IROperation
{
    Add, // Result = Value(Operand1) + Value(Operand2)
    Sub, // Result = Value(Operand1) - Value(Operand2)
    Mul, // Result = Value(Operand1) * Value(Operand2)
    Div, // Result = Value(Operand1) / Value(Operand2)
    Mod, // Result = Value(Operand1) % Value(Operand2)
    
    GetVar, // Result = Variable(Operand1)
    SetVar, // Variable(Operand1) = Value(Operand2)
    CreateVar, // Variable(Operand1) = Value(Operand2)
    
    Not, // Result = !Value(Operand1)
    And, // Result = Value(Operand1) && Value(Operand2)
    Or, // Result = Value(Operand1) || Value(Operand2)
    Xor, // Result = Value(Operand1) ^ Value(Operand2)
    
    Equal, // Result = Value(Operand1) == Value(Operand2)
    NotEqual, // Result = Value(Operand1) != Value(Operand2)
    GreaterThan, // Result = Value(Operand1) > Value(Operand2)
    GreaterThanOrEqual, // Result = Value(Operand1) >= Value(Operand2)
    LessThan, // Result = Value(Operand1) < Value(Operand2)
    LessThanOrEqual, // Result = Value(Operand1) <= Value(Operand2)
    
    Concat, // Result = Value(Operand1) + Value(Operand2)
    
    Constant, // Result = Data
    Print // Print(Value(Operand1))
}
