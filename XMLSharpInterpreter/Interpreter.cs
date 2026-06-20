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

    public void Run(IRProgram program, RunSettings settings)
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
        while (instructionIndex < program.Instructions.Length)
        {
            IRInstruction instruction = program.Instructions[instructionIndex];
            IOperationHandler handler = _handlers[instruction.Operation];

            if (settings.VerboseMode)
                Console.Write($"{instruction.Operation} ({instructionIndex}): \"");
            
            int delta = handler.Execute(instruction, instructionIndex, program.Constants, Registers, Variables, Parameters, CallStack, Functions, settings.VerboseMode);
            
            if (settings.VerboseMode)
                Console.WriteLine('\"');

            instructionIndex += delta;
        }
        
        sw.Stop();
        Console.WriteLine($"Execution completed in {sw.ElapsedMilliseconds}ms");
    }

    public static void Interpret(InterpreterSettings settings)
    {
        // ir.ReadFromFile will need to return an actual error at some point
        IIR ir = new IR();

        try {
            IRProgram? program = ir.ReadFromFile(settings.InputFile);

            if (program is null || program.Instructions.Length == 0)
            {
                Console.Error.WriteLine($"{settings.InputFile.Name} is empty or malformed.");
                Environment.Exit(1);
            }
            Interpreter interpreter = new();
            interpreter.Run(program, settings.ToRunSettings());
        } catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"An error occured while running {settings.InputFile.Name}: {e.Message}\n{e.StackTrace}");
            Console.ResetColor();
        }
    }

    public record RunSettings
    {
        public bool VerboseMode = false;
    }
}

public record InterpreterSettings
{
    public required FileInfo InputFile;
    
    public bool VerboseMode = false;

    internal Interpreter.RunSettings ToRunSettings()
    {
        return new Interpreter.RunSettings
        {
            VerboseMode = VerboseMode
        };
    }
}
