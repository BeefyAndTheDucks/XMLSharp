using Common;

namespace XMLSharpInterpreter;

public class Interpreter
{
    private readonly Dictionary<IROperation, IOperationHandler> _handlers = OperationHandlers.All.ToDictionary(h => h.Operation);
    internal Dictionary<int, dynamic> Registers { get; } = [];
    internal Dictionary<int, dynamic> Variables { get; } = [];

    public void Run(IRInstruction[] instructions, bool verboseMode)
    {
        // Validate that there exists a handler for each IROperation
        foreach (IROperation operation in Enum.GetValues<IROperation>())
        {
            if (!_handlers.ContainsKey(operation))
                throw new InvalidOperationException($"No handler for operation {operation}.");
        }
        
        int ip = 0;
        while (ip < instructions.Length)
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