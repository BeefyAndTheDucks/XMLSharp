using System.Diagnostics;
using System.Text;
using Common;

namespace XMLSharpInterpreter;

public class Interpreter
{
    private readonly Dictionary<IROperation, IOperationHandler> _handlers = OperationHandlers.All.ToDictionary(h => h.Operation);
    internal Stack<Dictionary<int, dynamic>> Registers { get; } = [];
    internal Stack<Dictionary<int, dynamic>> Variables { get; } = [];
    internal Stack<Dictionary<int, dynamic>> Parameters { get; } = [];
    private Stack<FunctionCall> CallStack { get; } = [];
    private Dictionary<int, int> Functions { get; } = []; // Key = function index, value = function info (begin index, result temp var)

    private int _debuggerCallstackPadding = 70;
    
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
        
        Registers.Push([]);
        Variables.Push([]);
        
        Stopwatch sw = Stopwatch.StartNew();
        
        int instructionIndex = 0;
        while (instructionIndex < program.Instructions.Length)
        {
            if (settings.Debugger)
                Console.Clear();
            
            IRInstruction instruction = program.Instructions[instructionIndex];
            IOperationHandler handler = _handlers[instruction.Operation];

            if (settings.VerboseMode)
                Console.Write($"{instruction.Operation} ({instructionIndex}): \"");
            
            int delta = handler.Execute(instruction, instructionIndex, program.Constants, Registers, Variables, Parameters, CallStack, Functions, settings.VerboseMode);
            
            if (settings.VerboseMode)
                Console.WriteLine('\"');
            
            if (settings.Debugger)
            {
                StringBuilder sb = new();
                
                for (int i = 0; i < program.Instructions.Length; i++)
                {
                    sb.Clear();
                    if (i == instructionIndex)
                        sb.Append("> ");
                    sb.Append($"[{i}] ");
                    IRInstruction instructionToPrint = program.Instructions[i];
                    sb.Append(instructionToPrint);
                    if (instructionToPrint.Operation == IROperation.GetParameter)
                    {
                        if (Parameters.Count > 0)
                            if (Parameters.Peek().TryGetValue(instructionToPrint.Operand1, out var parameterValue))
                                sb.Append($" // Parameters[{instructionToPrint.Operand1}] = {parameterValue}");
                    } else if (instructionToPrint.Operation == IROperation.GetVar)
                    {
                        if (Variables.Count > 0)
                            if (Variables.Peek().TryGetValue(instructionToPrint.Operand1, out var variableValue))
                                sb.Append($" // Variables[{instructionToPrint.Operand1}] = {variableValue}");
                    }
                    else
                    {
                        if (Parameters.Count > 0)
                            if (Registers.Peek().TryGetValue(instructionToPrint.Result, out var registerValue))
                                sb.Append($" // Registers[{instructionToPrint.Result}] = {registerValue}");
                    }
                    int padding = _debuggerCallstackPadding - sb.Length;
                    if (padding < 0)
                    {
                        _debuggerCallstackPadding = sb.Length + 5;
                        padding = 0;
                    }
                    sb.Append(new string(' ', padding));
                    sb.Append("| ");
                    if (i < CallStack.Count)
                    {
                        FunctionCall callStackEntry = CallStack.ElementAt(i);
                        sb.Append(callStackEntry.ReturnLocation);
                    }
                    Console.WriteLine(sb);
                }
                
                if (settings.DebuggerAutoStepRate is not null && settings.DebuggerAutoStepRate.Value > TimeSpan.Zero)
                    Thread.Sleep(settings.DebuggerAutoStepRate.Value);
                else
                    Console.ReadKey();
            }

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

    public static bool CanInterpret(FileInfo file)
    {
        IIR ir = new IR();
        return ir.IsIR(file);
    }

    public record RunSettings
    {
        public bool VerboseMode = false;
        public bool Debugger = false;

        public TimeSpan? DebuggerAutoStepRate;
    }
}

public record FunctionCall
{
    public int ReturnLocation;
}

public record InterpreterSettings
{
    public required FileInfo InputFile;
    
    public bool VerboseMode = false;
    public bool Debugger = false;

    public TimeSpan? DebuggerAutoStepRate;

    internal Interpreter.RunSettings ToRunSettings()
    {
        return new Interpreter.RunSettings
        {
            Debugger = Debugger,
            VerboseMode = VerboseMode,
            
            DebuggerAutoStepRate = DebuggerAutoStepRate
        };
    }
}
