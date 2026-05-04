using Common;

namespace XMLSharpInterpreter;

public class Interpreter
{
    private readonly Dictionary<IROperation, IOperationHandler> _handlers = OperationHandlers.All.ToDictionary(h => h.Operation);
    public Dictionary<int, dynamic> Registers { get; } = [];
    public Dictionary<int, dynamic> Variables { get; } = [];

    public void Run(IRInstruction[] instructions)
    {
        foreach (IRInstruction instruction in instructions)
        {
            IOperationHandler handler = _handlers[instruction.Operation];
            handler.Execute(instruction, Registers, Variables);
        }
    } 
}