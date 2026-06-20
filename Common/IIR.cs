using System.Text.Json.Serialization;

namespace Common;

public interface IIR
{
    IRProgram FromAst(AstNode ast);
    void WriteToFile(FileInfo file, IRProgram program);
    IRProgram? ReadFromFile(FileInfo file);
    bool IsIR(FileInfo file);
}

public record IRProgram(
    [property: JsonPropertyName("C")] IRConstant[] Constants,
    [property: JsonPropertyName("I")] IRInstruction[] Instructions
);

public record IRInstruction(
    [property: JsonPropertyName("O")] IROperation Operation,
    [property: JsonPropertyName("1")] int Operand1,
    [property: JsonPropertyName("2")] int Operand2,
    [property: JsonPropertyName("R")] int Result
)
{
    public override string ToString()
    {
        return $"({Operation}) {Operation switch
            {
                IROperation.Add => $"{Result} = {Operand1} + {Operand2}",
                IROperation.Sub => $"{Result} = {Operand1} - {Operand2}",
                IROperation.Mul => $"{Result} = {Operand1} * {Operand2}",
                IROperation.Div => $"{Result} = {Operand1} / {Operand2}",
                IROperation.Mod => $"{Result} = {Operand1} % {Operand2}",
                IROperation.GetVar => $"{Result} = var({Operand1})",
                IROperation.SetVar => $"var({Operand1}) = {Operand2}",
                IROperation.CreateVar => $"define var({Operand1}) = {Operand2}",
                IROperation.Not => $"{Result} = !{Operand1}",
                IROperation.And => $"{Result} = {Operand1} && {Operand2}",
                IROperation.Or => $"{Result} = {Operand1} || {Operand2}",
                IROperation.Xor => $"{Result} = {Operand1} ^ {Operand2}",
                IROperation.Equal => $"{Result} = {Operand1} == {Operand2}",
                IROperation.NotEqual => $"{Result} = {Operand1} != {Operand2}",
                IROperation.GreaterThan => $"{Result} = {Operand1} > {Operand2}",
                IROperation.GreaterThanOrEqual => $"{Result} = {Operand1} >= {Operand2}",
                IROperation.LessThan => $"{Result} = {Operand1} < {Operand2}",
                IROperation.LessThanOrEqual => $"{Result} = {Operand1} <= {Operand2}",
                IROperation.Concat => $"{Result} = \"{Operand1}\" concat \"{Operand2}\"",
                IROperation.Constant => $"{Result} = constant({Operand1})",
                IROperation.Print => $"print({Operand1})",
                IROperation.Jump => $"jump({Operand1} relative)",
                IROperation.If => $"if({Operand1}) jump(1 relative) else jump(2 relative)",
                IROperation.DefineFunction => $"define function(id = {Operand1})",
                IROperation.CallFunction => $"call function(id = {Operand1})",
                IROperation.Return => "return",
                IROperation.GetParameter => $"{Result} = parameter({Operand1})",
                IROperation.SetParameter => $"parameter({Result}) = {Operand1}",
                IROperation.Copy => $"{Result} = {Operand1}",
                IROperation.PrepareCallFunction => "prepare call function",
                _ => base.ToString() ?? throw new ArgumentOutOfRangeException()
            }}";
    }
}

public enum IRConstantKind : byte
{
    Number,
    Bool,
    Text,
    Decimal
}

public record IRConstant(
    [property: JsonPropertyName("K")] IRConstantKind Kind,
    [property: JsonPropertyName("V")] object Value
)
{
    public static IRConstant From(int value) => new(IRConstantKind.Number, value);
    public static IRConstant From(bool value) => new(IRConstantKind.Bool, value);
    public static IRConstant From(string value) => new(IRConstantKind.Text, value);
    public static IRConstant From(float value) => new(IRConstantKind.Decimal, value);
}

public enum IROperation : byte
{
    Add, // Result = Value(Operand1) + Value(Operand2)
    Sub, // Result = Value(Operand1) - Value(Operand2)
    Mul, // Result = Value(Operand1) * Value(Operand2)
    Div, // Result = Value(Operand1) / Value(Operand2)
    Mod, // Result = Value(Operand1) % Value(Operand2)
    
    GetVar, // Result = Variable(Operand1)
    SetVar, // Variable(Operand1) = Value(Operand2)
    CreateVar, // Variable(Operand1) = Value(Operand2)
    
    Not, // Result = !Value(Operand1)
    And, // Result = Value(Operand1) && Value(Operand2)
    Or, // Result = Value(Operand1) || Value(Operand2)
    Xor, // Result = Value(Operand1) ^ Value(Operand2)
    
    Equal, // Result = Value(Operand1) == Value(Operand2)
    NotEqual, // Result = Value(Operand1) != Value(Operand2)
    GreaterThan, // Result = Value(Operand1) > Value(Operand2)
    GreaterThanOrEqual, // Result = Value(Operand1) >= Value(Operand2)
    LessThan, // Result = Value(Operand1) < Value(Operand2)
    LessThanOrEqual, // Result = Value(Operand1) <= Value(Operand2)
    
    Concat, // Result = Value(Operand1) + Value(Operand2)
    
    Constant, // Result = Constant(Operand1)
    Print, // Print(Value(Operand1))
    
    Jump, // OperationIndex += Operand1
    
    If, // If(Value(Operand1)) OperationIndex += 1 else OperationIndex += 2     (If Value(Operand1) is number, it should be Value(Operand1) != 0, text should be !string.IsNullOrEmpty(Value(Operand1)))
    
    DefineFunction, // ID = Operand1
    PrepareCallFunction, // ParametersStack.Push(new parameters frame)
    CallFunction, // CallStack.Push(OperationIndex); OperationIndex = FunctionAddressAt(Operand1)
    Return, // OperationIndex = CallStack.Pop(); AND ParameterStack.Pop();
    
    GetParameter, // Result = ParameterStack.Peek()(Operand1)
    SetParameter, // ParameterStack.Peek()(Result) = Value(Operand1)
    
    Copy, // Result = Value(Operand1)
}
