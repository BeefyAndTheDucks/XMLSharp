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
    
    private readonly List<IRConstant> _constants = [];

    private readonly JsonSerializerOptions _options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    };

    private bool _dirty;
    
    public IRProgram FromAst(AstNode ast)
    {
        if (_dirty)
            throw new InvalidOperationException($"Cannot reuse IR instances after {nameof(FromAst)} has been called.");
        _dirty = true;
        
        _variableNameToVariableIndexTable.Clear();
        
        _temporaryValueIndex = 0;
        _variableIndex = 0;
        
        IRInstruction[] instructions = GenInstructions(ast);
        
        return new IRProgram(_constants.ToArray(), instructions);
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
                instructions.Add(new IRInstruction(IROperation.Constant, _constants.Count, 0, _temporaryValueIndex));
                _constants.Add(IRConstant.From(numberNode.Value));
                break;
            case DecimalNode decimalNode:
                instructions.Add(new IRInstruction(IROperation.Constant, _constants.Count, 0, _temporaryValueIndex));
                _constants.Add(IRConstant.From(decimalNode.Value));
                break;
            case TextNode textNode:
                instructions.Add(new IRInstruction(IROperation.Constant, _constants.Count, 0, _temporaryValueIndex));
                _constants.Add(IRConstant.From(textNode.Value));
                break;
            case BooleanNode booleanNode:
                instructions.Add(new IRInstruction(IROperation.Constant, _constants.Count, 0, _temporaryValueIndex));
                _constants.Add(IRConstant.From(booleanNode.Value));
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

    public void WriteToFile(FileInfo file, IRProgram program)
    {
        string json = JsonSerializer.Serialize(program, _options);
        File.WriteAllText(file.FullName, json);
    }

    public IRProgram? ReadFromFile(FileInfo file)
    {
        return JsonSerializer.Deserialize<IRProgram>(File.ReadAllText(file.FullName), _options);
    }
}
