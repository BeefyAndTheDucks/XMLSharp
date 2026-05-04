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
        new LessOrEqualHandler(),
        new AndHandler(),
        new OrHandler(),
        new XorHandler(),
        new ConcatHandler(),
        new NotHandler(),
        new JumpHandler(),
        new IfHandler()
    ];
}


// handle consts
internal class ConstantHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Constant;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = RequireData(instruction);
        return 1;
    }
}

// handle print
internal class PrintHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Print;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        Console.WriteLine(registers[instruction.Operand1]);
        return 1;
    }
}

// handle add
internal class AddHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Add;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1] + registers[instruction.Operand2];
        return 1;
    }
}

// handle sub
internal class SubHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Sub;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1] - registers[instruction.Operand2];
        return 1;
    }
}

// handle mul
internal class MulHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Mul;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1] * registers[instruction.Operand2];
        return 1;
    }
}

// handle div
internal class DivHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Div;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1] / registers[instruction.Operand2];
        return 1;
    }
}

// handle mod
internal class ModHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Mod;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1] % registers[instruction.Operand2];
        return 1;
    }
}

// handle creating variables
internal class CreateVarHandler : IOperationHandler
{
    public IROperation Operation => IROperation.CreateVar;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        if (variables is null) throw new InvalidOperationException("Variables table is required for variable operations.");

        variables[instruction.Operand1] = registers[instruction.Operand2];
        return 1;
    }
}

// handle setting variables
internal class SetVarHandler : IOperationHandler
{
    public IROperation Operation => IROperation.SetVar;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        if (variables is null) throw new InvalidOperationException("Variables table is required for variable operations.");

        variables[instruction.Operand1] = registers[instruction.Operand2];
        return 1;
    }
}

// handle getting variables
internal class GetVarHandler : IOperationHandler
{
    public IROperation Operation => IROperation.GetVar;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        if (variables is null) throw new InvalidOperationException("Variables table is required for variable operations.");

        registers[instruction.Result] = variables[instruction.Operand1];
        return 1;
    }
}

internal class EqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Equal;
    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1] == registers[instruction.Operand2];
        return 1;
    }
}

internal class NotEqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.NotEqual;
    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables = null)
    {
        registers[instruction.Result] = registers[instruction.Operand1] != registers[instruction.Operand2];
        return 1;
    }
}

internal class GreaterHandler : IOperationHandler
{
    public IROperation Operation => IROperation.GreaterThan;
    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables = null)
    {
        registers[instruction.Result] = registers[instruction.Operand1] > registers[instruction.Operand2];
        return 1;
    }
}

internal class GreaterOrEqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.GreaterThanOrEqual;
    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables = null)
    {
        registers[instruction.Result] = registers[instruction.Operand1] >= registers[instruction.Operand2];
        return 1;
    }
}

internal class LessHandler : IOperationHandler
{
    public IROperation Operation => IROperation.LessThan;
    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables = null)
    {
        registers[instruction.Result] = registers[instruction.Operand1] < registers[instruction.Operand2];
        return 1;
    }
}

internal class LessOrEqualHandler : IOperationHandler
{
    public IROperation Operation => IROperation.LessThanOrEqual;
    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables = null)
    {
        registers[instruction.Result] = registers[instruction.Operand1] <= registers[instruction.Operand2];
        return 1;
    }
}
// handle AND
internal class AndHandler : IOperationHandler
{
    public IROperation Operation => IROperation.And;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers, 
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1] && registers[instruction.Operand2];
        return 1;
    }
}

// handle OR
internal class OrHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Or;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1] || registers[instruction.Operand2];
        return 1;
    }
}

// handle XOR
internal class XorHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Xor;

    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1] ^ registers[instruction.Operand2];
        return 1;
    }
}

internal class ConcatHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Concat;
    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = registers[instruction.Operand1].ToString() + registers[instruction.Operand2].ToString();
        return 1;
    }
}

internal class NotHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Not;
    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        registers[instruction.Result] = !registers[instruction.Operand1];
        return 1;
    }
}

// jump handler
internal class JumpHandler : IOperationHandler
{
    public IROperation Operation => IROperation.Jump;
    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        return instruction.Operand1;
    }
}


// if handler
internal class IfHandler : IOperationHandler
{
    public IROperation Operation => IROperation.If;
    public int Execute(IRInstruction instruction, Dictionary<int, dynamic> registers,
        Dictionary<int, dynamic>? variables)
    {
        dynamic value = registers[instruction.Operand1];

        bool condition = value switch
        {
            bool b => b,
            int i => i != 0,
            string s => !string.IsNullOrEmpty(s),
            null => false,
            _ => true
        };

        return condition ? 1 : 2;
    }
}
