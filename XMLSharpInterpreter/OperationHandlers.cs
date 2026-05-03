using Common;
using static XMLSharpInterpreter.InstructionHelper;

namespace XMLSharpInterpreter;



// remember to add new handlers to OperationHandlers!
public static class OperationHandlers
{
    public static readonly IOperationHandler[] All = [
        new AddHandler(),
        new SubHandler(),
        new MulHandler(),
        new DivHandler(),
        new ModHandler(),
        new ConstantHandler(),
        new PrintHandler(),
        new CreateVarHandler(),
        new SetVarHandler(),
        new GetVarHandler(),
    ];
}


// handle consts
class ConstantHandler : IOperationHandler
{
    public IROperation Operation { get; } = IROperation.Constant;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable)
    {
        registers[instruction.Result] = RequireData(instruction);
    }
}

// handle print
class PrintHandler : IOperationHandler
{
    public IROperation Operation { get; } = IROperation.Print;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable)
    {
        Console.WriteLine(registers[instruction.Operand1]);
    }
}

// handle add
class AddHandler : IOperationHandler
{
    public IROperation Operation { get; } = IROperation.Add;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] + (int)registers[instruction.Operand2];
    }
}

// handle sub
class SubHandler : IOperationHandler
{
    public IROperation Operation { get; } = IROperation.Sub;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] - (int)registers[instruction.Operand2];
    }
}

// handle mul
class MulHandler : IOperationHandler
{
    public IROperation Operation { get; } = IROperation.Mul;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] * (int)registers[instruction.Operand2];
    }
}

// handle div
class DivHandler : IOperationHandler
{
    public IROperation Operation { get; } = IROperation.Div;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] / (int)registers[instruction.Operand2];
    }
}

// handle mod
class ModHandler : IOperationHandler
{
    public IROperation Operation { get; } = IROperation.Mod;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] % (int)registers[instruction.Operand2];
    }
}

// handle creating variables
class CreateVarHandler : IOperationHandler
{
    public IROperation Operation { get; } = IROperation.CreateVar;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable)
    {
        if (symbolTable is null) throw new InvalidOperationException("Symbol table is required for variable operations.");

        var name = (string)RequireData(instruction);
        symbolTable[name] = instruction.Operand1;
    }
}

// handle setting variables
class SetVarHandler : IOperationHandler
{
    public IROperation Operation { get; } = IROperation.SetVar;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable)
    {
        if (symbolTable is null) throw new InvalidOperationException("Symbol table is required for variable operations.");

        var name = (string)RequireData(instruction);
        int slot = symbolTable[name];
        registers[slot] = registers[instruction.Operand1];
    }
}

// handle getting variables
class GetVarHandler : IOperationHandler
{
    public IROperation Operation { get; } = IROperation.GetVar;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<string, int>? symbolTable)
    {
        if (symbolTable is null) throw new InvalidOperationException("Symbol table is required for variable operations.");

        var name = (string)RequireData(instruction);
        int slot = symbolTable[name];
        registers[instruction.Result] = registers[slot];
    }
}