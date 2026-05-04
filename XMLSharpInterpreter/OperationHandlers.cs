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
        new EqualHandler(),
        new NotEqualHandler(),
        new GreaterHandler(),
        new GreaterOrEqualHandler(),
        new LessHandler(),
        new LessOrEqualHandler()
    ];
}


// handle consts
internal class ConstantHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Constant;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        registers[instruction.Result] = RequireData(instruction);
    }
}

// handle print
internal class PrintHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Print;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        Console.WriteLine(registers[instruction.Operand1]);
    }
}

// handle add
internal class AddHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Add;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] + (int)registers[instruction.Operand2];
    }
}

// handle sub
internal class SubHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Sub;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] - (int)registers[instruction.Operand2];
    }
}

// handle mul
internal class MulHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Mul;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] * (int)registers[instruction.Operand2];
    }
}

// handle div
internal class DivHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Div;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] / (int)registers[instruction.Operand2];
    }
}

// handle mod
internal class ModHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Mod;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] % (int)registers[instruction.Operand2];
    }
}

// handle creating variables
internal class CreateVarHandler : IOperationHandler
{
    public IROperation Operation => IROperation.CreateVar;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        if (variables is null) throw new InvalidOperationException("Variables table is required for variable operations.");

        variables[instruction.Operand1] = registers[instruction.Operand2];
    }
}

// handle setting variables
internal class SetVarHandler : IOperationHandler
{
    public IROperation Operation => IROperation.SetVar;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        if (variables is null) throw new InvalidOperationException("Variables table is required for variable operations.");

        variables[instruction.Operand1] = registers[instruction.Operand2];
    }
}

// handle getting variables
internal class GetVarHandler : IOperationHandler
{
    public IROperation Operation => IROperation.GetVar;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        if (variables is null) throw new InvalidOperationException("Variables table is required for variable operations.");

        registers[instruction.Result] = variables[instruction.Operand1];
    }
}

internal class EqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Equal;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1].Equals(registers[instruction.Operand2]);
    }
}

internal class NotEqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.NotEqual;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<int, object>? variables = null)
    {
        registers[instruction.Result] = !registers[instruction.Operand1].Equals(registers[instruction.Operand2]);
    }
}

internal class GreaterHandler : IOperationHandler
{
    public IROperation Operation => IROperation.GreaterThan;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<int, object>? variables = null)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] > (int)registers[instruction.Operand2];
    }
}

internal class GreaterOrEqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.GreaterThanOrEqual;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<int, object>? variables = null)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] >= (int)registers[instruction.Operand2];
    }
}

internal class LessHandler : IOperationHandler
{
    public IROperation Operation => IROperation.LessThan;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<int, object>? variables = null)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] < (int)registers[instruction.Operand2];
    }
}

internal class LessOrEqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.LessThanOrEqual;
    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, Dictionary<int, object>? variables = null)
    {
        registers[instruction.Result] = (int)registers[instruction.Operand1] <= (int)registers[instruction.Operand2];
    }
}
// handle AND
internal class AndHandler : IOperationHandler
{
    public IROperation Operation => IROperation.And;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers, 
        Dictionary<int, object>? variables)
    {
        registers[instruction.Result] = (bool)registers[instruction.Operand1] && (bool)registers[instruction.Operand2];
    }
}

// handle OR
internal class OrHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Or;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        registers[instruction.Result] = (bool)registers[instruction.Operand1] || (bool)registers[instruction.Operand2];
    }
}

// handle XOR
internal class XorHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Xor;

    public void Execute(IRInstruction instruction, Dictionary<int, object> registers,
        Dictionary<int, object>? variables)
    {
        registers[instruction.Result] = (bool)registers[instruction.Operand1] ^ (bool)registers[instruction.Operand2];
    }
}
