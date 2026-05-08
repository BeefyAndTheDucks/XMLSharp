using System.Diagnostics;
using Common;

namespace XMLSharpInterpreter;

public class Interpreter
{
    private readonly Dictionary<IROperation, IOperationHandler> _handlers = OperationHandlers.All.ToDictionary(h => h.Operation);
    internal Dictionary<int, dynamic> Registers { get; } = [];
    internal Dictionary<int, dynamic> Variables { get; } = [];
    internal Dictionary<int, dynamic> Parameters { get; } = [];
    private Stack<int> CallStack { get; } = [];
    private Dictionary<int, int> Functions { get; } = []; // Key = function index, value = function info (begin index, result temp var)

    public void Run(IRInstruction[] instructions, bool verboseMode)
    {
        // Validate that there exists a handler for each IROperation
        var missingHandlers = Enum.GetValues<IROperation>().Where(h => !_handlers.ContainsKey(h)).ToList();
        if (missingHandlers.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING: Missing handlers for operations: {string.Join(", ", missingHandlers)}\n\n\n");
            Console.ResetColor();
        }
        
        Stopwatch sw = Stopwatch.StartNew();
        
        int instructionIndex = 0;
        while (instructionIndex < instructions.Length)
        {
            IRInstruction instruction = instructions[instructionIndex];
            IOperationHandler handler = _handlers[instruction.Operation];

            if (verboseMode)
                Console.Write($"{instruction.Operation} ({instructionIndex}): \"");
            
            int delta = handler.Execute(instruction, instructionIndex, Registers, Variables, Parameters, CallStack, Functions, verboseMode);
            
            if (verboseMode)
                Console.WriteLine('\"');

            instructionIndex += delta;
        }
        
        sw.Stop();
        Console.WriteLine($"Execution completed in {sw.ElapsedMilliseconds}ms");
    } 
}