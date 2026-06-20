using Common;

namespace XMLSharpInterpreter;

public interface IOperationHandler
{
    IROperation Operation { get; }
    int Execute(
        IRInstruction instruction, 
        int instructionIndex,
        IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers, 
        Stack<Dictionary<int, dynamic>> variables,
        Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions,
        bool verboseMode);

}
