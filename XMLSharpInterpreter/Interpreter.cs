using Common;

namespace XMLSharpInterpreter;

public class Interpreter
{
    private readonly Dictionary<IROperation, IOperationHandler> _handlers = OperationHandlers.All.ToDictionary(h => h.Operation);
    public Dictionary<int, dynamic> Registers { get; } = [];
    public Dictionary<int, dynamic> Variables { get; } = [];

    public void Run(IRInstruction[] instructions, bool verboseMode)
    {
        int ip = 0;
        while (ip < instructions.Length - 1)
        {
            IRInstruction instruction = instructions[ip];
            IOperationHandler handler = _handlers[instruction.Operation];

            if (verboseMode)
                Console.Write($"{instruction.Operation} ({ip}): \"");
            
            int delta = handler.Execute(instruction, Registers, Variables, verboseMode);
            
            if (verboseMode)
                Console.WriteLine('\"');

            ip += delta;
        }
    } 
}