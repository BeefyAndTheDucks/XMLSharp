using Common;

namespace XMLSharpInterpreter;

public interface IOperationHandler
{
    IROperation Operation { get; }
    void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<int, object>? variables = null);

}
