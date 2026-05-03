using Common;

namespace XMLSharpInterpreter;

public class Interpreter
{
    Dictionary<IROperation, IOperationHandler> handlers = OperationHandlers.All.ToDictionary(h => h.Operation);
    public Dictionary<int, object> Registers { get; } = [];
    public Dictionary<string, int> SymbolTable { get; } = [];

    public void Run(IRInstruction[] instructions)
    {
        foreach (IRInstruction instruction in instructions)
        {
            IOperationHandler handler = handlers[instruction.Operation];
            handler.Execute(instruction, Registers, SymbolTable);
        }
    } 
}