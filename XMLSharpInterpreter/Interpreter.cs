using Common;

namespace XMLSharpInterpreter;

public class Interpreter
{
    private readonly Dictionary<IROperation, IOperationHandler> _handlers = OperationHandlers.All.ToDictionary(h => h.Operation);
    public Dictionary<int, object> Registers { get; } = [];
    public Dictionary<int, object> Variables { get; } = [];

    public void Run(IRInstruction[] instructions)
    {
        foreach (IRInstruction instruction in instructions)
        {
            IOperationHandler handler = _handlers[instruction.Operation];
            handler.Execute(instruction, Registers, Variables);
        }
    } 
}