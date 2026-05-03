namespace Common;

// IR stands for Intermediate Representation. We convert our AST nodes to an IR representation, which we then write to a file to let an interpreter/compiler use.
public class IR : IIR
{
    public IRInstruction[] FromAst(AstNode[] ast)
    {
        throw new NotImplementedException();
    }

    public void WriteToFile(string path, IRInstruction[] instructions)
    {
        throw new NotImplementedException();
    }

    public IRInstruction[] ReadFromFile(string path)
    {
        throw new NotImplementedException();
    }
}

public record IRInstruction(IROperation Operation, int Operand1, int Operand2, int Result);

public enum IROperation
{
    Add,
    Sub,
    Mul,
    Div,
    Mod,
    GetVar,
    SetVar,
    CreateVar,
    Print
}
