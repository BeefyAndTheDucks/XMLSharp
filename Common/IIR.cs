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
);

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
    CallFunction, // CallStack.Push(OperationIndex); OperationIndex = FunctionAddressAt(Operand1)
    Return, // OperationIndex = CallStack.Pop();
    
    GetParameter, // Result = Parameter(Operand1)
    SetParameter, // Parameter(Operand1) = Value(Operand2)
    
    Copy, // Result = Value(Operand1)
}

