using Common;

namespace XMLSharpInterpreter;

public interface IOperationHandler
{
    IROperation Operation { get; }
    void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable = null);

}
