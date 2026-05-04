using Common;

namespace XMLSharpInterpreter;

public class Interpreter
{
    private readonly Dictionary<IROperation, IOperationHandler> _handlers = OperationHandlers.All.ToDictionary(h => h.Operation);
    public Dictionary<int, dynamic> Registers { get; } = [];
    public Dictionary<int, dynamic> Variables { get; } = [];

    public void Run(IRInstruction[] instructions)
    {
        int ip = 0;
        while (ip < instructions.Count() - 1)
        {
            IRInstruction instruction = instructions[ip];
            IOperationHandler handler = _handlers[instruction.Operation];

            int delta = handler.Execute(instruction, Registers, Variables);

            ip += delta;
        }
    } 
}