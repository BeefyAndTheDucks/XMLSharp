using Common;

namespace XMLSharpInterpreter;

public interface IOperationHandler
{
    IROperation Operation { get; }
    int Execute(
        IRInstruction instruction, 
        int instructionIndex,
        IRConstant[] constants,
        Dictionary<int, dynamic> registers, 
        Dictionary<int, dynamic> variables,
        Dictionary<int, dynamic> parameters,
        Stack<int> callStack,
        Dictionary<int, int> functions,
        bool verboseMode);

}
