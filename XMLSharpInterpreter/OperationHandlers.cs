using System.Reflection;
using Common;
using JetBrains.Annotations;
using static XMLSharpInterpreter.InstructionHelper;

namespace XMLSharpInterpreter;

public static class OperationHandlers
{
    public static IOperationHandler[] All
    {
        get
        {
            if (field is not null)
                return field;
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            field = assembly.GetTypes()
                .Where(t => typeof(IOperationHandler).IsAssignableFrom(t) &&
                            t is { IsAbstract: false, IsInterface: false })
                .Select(t => Activator.CreateInstance(t) as IOperationHandler)
                .Where(inst => inst != null)
                .ToArray()!;
            return field;
        }
    }
}


// handle consts
[UsedImplicitly]
internal class ConstantHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Constant;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = GetConstantValue(constants[instruction.Operand1]);
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

// handle print
[UsedImplicitly]
internal class PrintHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Print;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        if (verboseMode)
            Console.Write(registers.Peek()[instruction.Operand1]);
        else
            Console.WriteLine(registers.Peek()[instruction.Operand1]);
        
        return 1;
    }
}

// handle add
[UsedImplicitly]
internal class AddHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Add;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] + registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");

        return 1;
    }
}

// handle sub
[UsedImplicitly]
internal class SubHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Sub;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] - registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");

        return 1;
    }
}

// handle mul
[UsedImplicitly]
internal class MulHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Mul;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] * registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");

        return 1;
    }
}

// handle div
[UsedImplicitly]
internal class DivHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Div;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] / registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");

        return 1;
    }
}

// handle mod
[UsedImplicitly]
internal class ModHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Mod;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] % registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

// handle creating variables
[UsedImplicitly]
internal class CreateVarHandler : IOperationHandler
{
    public IROperation Operation => IROperation.CreateVar;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        if (variables is null) throw new InvalidOperationException("Variables table is required for variable operations.");

        variables.Peek()[instruction.Operand1] = registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{variables.Peek()[instruction.Operand1]}");
        
        return 1;
    }
}

// handle setting variables
[UsedImplicitly]
internal class SetVarHandler : IOperationHandler
{
    public IROperation Operation => IROperation.SetVar;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        if (variables is null) throw new InvalidOperationException("Variables table is required for variable operations.");

        variables.Peek()[instruction.Operand1] = registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{variables.Peek()[instruction.Operand1]}");
        
        return 1;
    }
}

// handle getting variables
[UsedImplicitly]
internal class GetVarHandler : IOperationHandler
{
    public IROperation Operation => IROperation.GetVar;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        if (variables is null) throw new InvalidOperationException("Variables table is required for variable operations.");

        registers.Peek()[instruction.Result] = FindVariableValue(instruction.Operand1, variables);
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class EqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Equal;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] == registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class NotEqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.NotEqual;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] != registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class GreaterHandler : IOperationHandler
{
    public IROperation Operation => IROperation.GreaterThan;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] > registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class GreaterOrEqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.GreaterThanOrEqual;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] >= registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class LessHandler : IOperationHandler
{
    public IROperation Operation => IROperation.LessThan;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] < registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class LessOrEqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.LessThanOrEqual;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] <= registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}
// handle AND
[UsedImplicitly]
internal class AndHandler : IOperationHandler
{
    public IROperation Operation => IROperation.And;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] && registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

// handle OR
[UsedImplicitly]
internal class OrHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Or;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] || registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

// handle XOR
[UsedImplicitly]
internal class XorHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Xor;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1] ^ registers.Peek()[instruction.Operand2];
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class ConcatHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Concat;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1].ToString() + registers.Peek()[instruction.Operand2].ToString();
        
        if (verboseMode)
            Console.Write($"{registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class NotHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Not;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        if (verboseMode)
            Console.Write($"{!registers.Peek()[instruction.Operand1]}");
        
        registers.Peek()[instruction.Result] = !registers.Peek()[instruction.Operand1];
        return 1;
    }
}

// jump handler
[UsedImplicitly]
internal class JumpHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Jump;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        if (verboseMode)
            Console.Write($"{instruction.Operand1}");
        
        return instruction.Operand1;
    }
}


// if handler
[UsedImplicitly]
internal class IfHandler : IOperationHandler
{
    public IROperation Operation => IROperation.If;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        dynamic value = registers.Peek()[instruction.Operand1];

        bool condition = value switch
        {
            bool b => b,
            int i => i != 0,
            string s => !string.IsNullOrEmpty(s),
            null => false,
            _ => true
        };
        
        if (verboseMode)
            Console.Write(condition);

        return condition ? 1 : 2;
    }
}

[UsedImplicitly]
internal class FunctionDefinitionHandler : IOperationHandler
{
    public IROperation Operation => IROperation.DefineFunction;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables, Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack,
        Dictionary<int, int> functions, bool verboseMode)
    {
        functions[instruction.Operand1] = instructionIndex;
        
        if (verboseMode)
            Console.Write($"{instruction.Operand1}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class SetParameterHandler : IOperationHandler
{
    public IROperation Operation => IROperation.SetParameter;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables,
        Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack, Dictionary<int, int> functions, bool verboseMode)
    {
        parameters.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1];
        
        if (verboseMode)
            Console.Write($"param {instruction.Result} = {registers.Peek()[instruction.Operand1]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class FunctionCallHandler : IOperationHandler
{
    public IROperation Operation => IROperation.CallFunction;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables,
        Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack, Dictionary<int, int> functions, bool verboseMode)
    {
        FunctionCall functionCall = new()
        {
            ReturnLocation = instructionIndex + 1,
        };
        callStack.Push(functionCall);
        
        registers.Push([]);
        variables.Push([]);
        
        int functionCallIndex = instruction.Operand1;
        int functionCallBeginIndex = functions[functionCallIndex] + 2;
        
        int instructionDelta = JumpTo(functionCallBeginIndex, instructionIndex);
        
        if (verboseMode)
            Console.Write($"call {functionCallIndex} at instruction {functionCallBeginIndex} (delta: {instructionDelta})");
        
        return instructionDelta;
    }
}

[UsedImplicitly]
internal class GetParameterHandler : IOperationHandler
{
    public IROperation Operation => IROperation.GetParameter;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables,
        Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack, Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = parameters.Peek()[instruction.Operand1];
        
        if (verboseMode)
            Console.Write($"register {instruction.Result} = {parameters.Peek()[instruction.Operand1]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class ReturnValueHandler : IOperationHandler
{
    public IROperation Operation => IROperation.ReturnValue;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables,
        Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack, Dictionary<int, int> functions, bool verboseMode)
    {
        parameters.Pop();
        var functionRegisters = registers.Pop();
        registers.Peek()[instruction.Result] = functionRegisters[instruction.Operand1];
        variables.Pop();
        FunctionCall functionCall = callStack.Pop();
        return JumpTo(functionCall.ReturnLocation, instructionIndex);
    }
}

[UsedImplicitly]
internal class ReturnVoidHandler : IOperationHandler
{
    public IROperation Operation => IROperation.ReturnVoid;
    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers,
        Stack<Dictionary<int, dynamic>> variables,
        Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack, Dictionary<int, int> functions, bool verboseMode)
    {
        parameters.Pop();
        registers.Pop();
        variables.Pop();
        FunctionCall functionCall = callStack.Pop();
        return JumpTo(functionCall.ReturnLocation, instructionIndex);
    }
}

[UsedImplicitly]
internal class CopyHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Copy;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers, Stack<Dictionary<int, dynamic>> variables,
        Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack, Dictionary<int, int> functions, bool verboseMode)
    {
        registers.Peek()[instruction.Result] = registers.Peek()[instruction.Operand1];
        
        if (verboseMode)
            Console.Write($"{instruction.Result} = {registers.Peek()[instruction.Result]}");
        
        return 1;
    }
}

[UsedImplicitly]
internal class PrepareCallFunctionHandler : IOperationHandler
{
    public IROperation Operation => IROperation.PrepareCallFunction;

    public int Execute(IRInstruction instruction, int instructionIndex, IRConstant[] constants,
        Stack<Dictionary<int, dynamic>> registers, Stack<Dictionary<int, dynamic>> variables,
        Stack<Dictionary<int, dynamic>> parameters,
        Stack<FunctionCall> callStack, Dictionary<int, int> functions, bool verboseMode)
    {
        parameters.Push([]);
        
        if (verboseMode)
            Console.Write("push new parameters frame");
        
        return 1;
    }
}
