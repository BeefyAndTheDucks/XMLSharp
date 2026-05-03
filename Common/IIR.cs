namespace Common;

public interface IIR
{
    IRInstruction[] FromAst(AstNode[] ast);
    void WriteToFile(FileInfo file, IRInstruction[] instructions);
    IRInstruction[]? ReadFromFile(FileInfo file);
}