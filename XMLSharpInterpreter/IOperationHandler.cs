using Common;

namespace XMLSharpInterpreter;

public interface IOperationHandler
{
    IROperation Operation { get; }
    int Execute(
        IRInstruction instruction, 
        Dictionary<int, dynamic> registers, 
        Dictionary<int, dynamic> variables,
        bool verboseMode);

}
