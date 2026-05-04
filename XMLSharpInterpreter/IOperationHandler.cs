using Common;

namespace XMLSharpInterpreter;

public interface IOperationHandler
{
    IROperation Operation { get; }
    void Execute(IRInstruction instruction, Dictionary<int, dynamic> registers, Dictionary<int, dynamic>? variables = null);

}
