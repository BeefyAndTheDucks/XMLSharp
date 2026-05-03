namespace Common;

public interface IIR
{
    IRInstruction[] FromAst(AstNode[] ast);
    void WriteToFile(string path, IRInstruction[] instructions);
    IRInstruction[] ReadFromFile(string path);
}