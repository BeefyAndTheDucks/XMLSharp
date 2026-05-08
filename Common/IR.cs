using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common;

// IR stands for Intermediate Representation. We convert our AST nodes to an IR representation, which we then write to a file to let an interpreter/compiler use.
public class IR : IIR
{
    private int _temporaryValueIndex;
    private int _variableIndex;
    private int _functionIndex;
    
    private readonly Dictionary<string, int> _variableNameToVariableIndexTable = new();
    private readonly Dictionary<string, int> _functionNameToFunctionIndexTable = new();
    private readonly Dictionary<string, int> _functionNameToOutputTemporaryTable = new();
    private readonly Dictionary<string, string[]> _functionParameters = [];
    private readonly Stack<string> _currentFunctionName = [];

    private readonly JsonSerializerOptions _options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    };

    private bool _dirty;
    
    public IRInstruction[] FromAst(AstNode ast)
    {
        if (_dirty)
            throw new InvalidOperationException($"Cannot reuse IR instances after {nameof(FromAst)} has been called.");
        _dirty = true;
        
        _variableNameToVariableIndexTable.Clear();
        
        _temporaryValueIndex = 0;
        _variableIndex = 0;
        
        return GenInstructions(ast);
    }

    private IRInstruction[] GenInstructions(AstNode node)
    {
        List<IRInstruction> instructions = [];

        switch (node)
        {
            // Blocks
            case BlockNode blockNode:
                foreach (AstNode subNode in blockNode.Nodes)
                    instructions.AddRange(GenInstructions(subNode));
                break;
            
            // Variables
            case CreateVariableNode createVariableNode:
                instructions.AddRange(GenInstructions(createVariableNode.ValueNode));
                _variableNameToVariableIndexTable.Add(createVariableNode.Name, _variableIndex++);
                instructions.Add(new IRInstruction(IROperation.CreateVar, _variableNameToVariableIndexTable[createVariableNode.Name], _temporaryValueIndex++, 0));
                break;
            case GetVariableNode getVariableNode:
                instructions.Add(new IRInstruction(IROperation.GetVar, _variableNameToVariableIndexTable[getVariableNode.Name], 0, _temporaryValueIndex));
                break;
            case SetVariableNode setVariableNode:
                instructions.AddRange(GenInstructions(setVariableNode.ValueNode));
                instructions.Add(new IRInstruction(IROperation.SetVar, _variableNameToVariableIndexTable[setVariableNode.Name], _temporaryValueIndex++, 0));
                break;
            
            // Datatypes/Constants
            case NumberNode numberNode:
                instructions.Add(new IRInstruction(IROperation.Constant, 0, 0, _temporaryValueIndex, numberNode.Value));
                break;
            case DecimalNode decimalNode:
                instructions.Add(new IRInstruction(IROperation.Constant, 0, 0, _temporaryValueIndex, decimalNode.Value));
                break;
            case TextNode textNode:
                instructions.Add(new IRInstruction(IROperation.Constant, 0, 0, _temporaryValueIndex, textNode.Value));
                break;
            case BooleanNode booleanNode:
                instructions.Add(new IRInstruction(IROperation.Constant, 0, 0, _temporaryValueIndex, booleanNode.Value));
                break;
            
            // Arithmetics
            case AstNodeWithLeftRight lrNode:
            {
                instructions.AddRange(GenInstructions(lrNode.LeftNode));
                int leftSideIndex = _temporaryValueIndex;
                _temporaryValueIndex++;
                instructions.AddRange(GenInstructions(lrNode.RightNode));
                instructions.Add(new IRInstruction(lrNode.IrOperation, leftSideIndex, _temporaryValueIndex++, _temporaryValueIndex));
                break;
            }

            case AstNodeWithSingleChild singleChildNode:
                instructions.AddRange(GenInstructions(singleChildNode.Child));
                instructions.Add(new IRInstruction(singleChildNode.IrOperation, _temporaryValueIndex++, 0, _temporaryValueIndex));
                break;
            
            // Control flow
            
            /*
             * If statements are handled a bit weird. To avoid using operands, we jump to the next instruction if true, or the one after if false. To allow for big blocks, we insert
             * Jump statements to jump to the correct block.
             * Here's the pseudocode:
             * If
             *   True -> Jump to TrueBlock
             *   False -> Jump to FalseBlock
             *
             *   TrueBlock:
             *     Whatever Instructions
             *     Jump to FinishedBlock
             *
             *   FalseBlock:
             *     Whatever Instructions (can even be empty)
             *     (implicit jump to FinishedBlock)
             *
             *   FinishedBlock:
             *     Whatever Instructions
             *
             * A possible optimization would be to use the Operand2 and Result to store the TrueBlock and FalseBlock, but it could be messy in case there's no FalseBlock.
             * Another idea is to place FalseBlock instead of the jump to FalseBlock and then jumping to FinishedBlock. This removes the Jump to FinishedBlock in TrueBlock,
             * and the Jump to FalseBlock. Currently, I don't have the energy to do such optimization.
             */
            case IfNode ifNode:
            {
                bool hasIfFalse = ifNode.IfFalse != null;
                
                instructions.AddRange(GenInstructions(ifNode.Condition));
                instructions.Add(new IRInstruction(IROperation.If, _temporaryValueIndex++, 0, 0));
                instructions.Add(new IRInstruction(IROperation.Jump, 2, 0, 0)); // Jump 2 steps ahead, that's where the IfTrue block starts.
                IRInstruction[] ifTrue = GenInstructions(ifNode.IfTrue);
                IRInstruction[] ifFalse = [];
                if (hasIfFalse)
                    ifFalse = GenInstructions(ifNode.IfFalse!);
                
                instructions.Add(new IRInstruction(IROperation.Jump, ifTrue.Length + (hasIfFalse ? 2 : 1), 0, 0)); // Jump to after the IfTrue block.
                
                instructions.AddRange(ifTrue);
                if (hasIfFalse)
                    instructions.Add(new IRInstruction(IROperation.Jump, ifFalse.Length + 1, 0, 0));
                
                instructions.AddRange(ifFalse);
                
                break;
            }

            case WhileNode whileNode:
            {
                var conditionInstructions = GenInstructions(whileNode.Condition);
                instructions.AddRange(conditionInstructions);
                instructions.Add(new IRInstruction(IROperation.If, _temporaryValueIndex++, 0, 0));
                instructions.Add(new IRInstruction(IROperation.Jump, 2, 0, 0));

                var loopedInstructions = GenInstructions(whileNode.Loop);
                
                instructions.Add(new IRInstruction(IROperation.Jump, loopedInstructions.Length + 2, 0, 0));
                
                instructions.AddRange(loopedInstructions);
                
                instructions.Add(new IRInstruction(IROperation.Jump, -loopedInstructions.Length - conditionInstructions.Length - 3, 0, 0));
                break;
            }
            
            // Functions
            case FunctionNode functionNode:
            {
                int functionIndex = _functionIndex++;
                _currentFunctionName.Push(functionNode.Name);
                _functionParameters[functionNode.Name] = functionNode.ParameterNames;
                _functionNameToFunctionIndexTable.Add(functionNode.Name, functionIndex);
                var contentInstructions = GenInstructions(functionNode.Contents);
                _currentFunctionName.Pop();
                
                instructions.Add(new IRInstruction(IROperation.DefineFunction, functionIndex, 0, 0));
                instructions.Add(new IRInstruction(IROperation.Jump, contentInstructions.Length + 1, 0, 0)); // Jump over the function contents
                instructions.AddRange(contentInstructions);
                _functionNameToOutputTemporaryTable.Add(functionNode.Name, _temporaryValueIndex - 1);
                _temporaryValueIndex++;
                
                break;
            }

            case GetParameterNode getParameterNode:
            {
                instructions.Add(new IRInstruction(IROperation.GetParameter, _functionParameters[_currentFunctionName.Peek()].IndexOf(getParameterNode.Name), 0, _temporaryValueIndex));
                break;
            }

            case CallFunctionNode callFunctionNode:
            {
                for (int parameterIndex = 0; parameterIndex < _functionParameters[callFunctionNode.Name].Length; parameterIndex++)
                {
                    instructions.AddRange(GenInstructions(callFunctionNode.Arguments[parameterIndex]));
                    instructions.Add(new IRInstruction(IROperation.SetParameter, parameterIndex, _temporaryValueIndex++, 0));
                }
                
                instructions.Add(new IRInstruction(IROperation.CallFunction, _functionNameToFunctionIndexTable[callFunctionNode.Name], 0, 0));
                instructions.Add(new IRInstruction(IROperation.Copy, _functionNameToOutputTemporaryTable[callFunctionNode.Name], 0, _temporaryValueIndex));
                break;
            }

            case VoidReturnNode:
                instructions.Add(new IRInstruction(IROperation.Return, 0, 0, 0));
                break;

            default:
                throw new NotSupportedException(node.GetType().Name + " is not supported yet by the IR.");
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

public enum IROperation : byte
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
    Print, // Print(Value(Operand1))
    
    Jump, // OperationIndex += Operand1
    
    If, // If(Value(Operand1)) OperationIndex += 1 else OperationIndex += 2     (If Value(Operand1) is number, it should be Value(Operand1) != 0, text should be !string.IsNullOrEmpty(Value(Operand1)))
    
    DefineFunction, // ID = Operand1
    CallFunction, // CallStack.Push(OperationIndex); OperationIndex = FunctionAddressAt(Operand1)
    Return, // OperationIndex = CallStack.Pop();
    
    GetParameter, // Result = Parameter(Operand1)
    SetParameter, // Parameter(Operand1) = Value(Operand2)
    
    Copy, // Result = Value(Operand1)
}
