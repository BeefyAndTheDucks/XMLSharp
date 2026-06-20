using Common;
using XMLSharpInterpreter;

namespace XMLSharpInterpreterTests;



[TestFixture]
[TestOf(typeof(Interpreter))]
public class InterpreterTest
{
    private static readonly Interpreter.RunSettings RunSettings = new()
    {
        VerboseMode = true
    };
    
    private static (Dictionary<int, dynamic> Registers, Dictionary<int, dynamic> Variables) Run(IRInstruction[] instructions, IRConstant[]? constants = null)
    {
        Interpreter interpreter = new();
        interpreter.Run(new IRProgram(constants ?? [], instructions), RunSettings);
        return (interpreter.Registers, interpreter.Variables);
    }

    [Test]
    public void TestConstant()
    {
        var (registers, _) = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0)
        ], [
            IRConstant.From(42)
        ]);
        Assert.That(registers[0], Is.EqualTo(42));
    }

    [Test]
    public void TestAdd()
    {
        // t0 = 5, t1 = 3, t2 = t0 + t1
        var (registers, _) = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0),
            new IRInstruction(IROperation.Constant, 1, 0, 1),
            new IRInstruction(IROperation.Add,      0, 1, 2)
        ], [
            IRConstant.From(5),
            IRConstant.From(3)
        ]);
        Assert.That(registers[2], Is.EqualTo(8));
    }

    [Test]
    public void TestSub()
    {
        var (registers, _) = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0),
            new IRInstruction(IROperation.Constant, 1, 0, 1),
            new IRInstruction(IROperation.Sub,      0, 1, 2)
        ], [
            IRConstant.From(10),
            IRConstant.From(4)
        ]);
        Assert.That(registers[2], Is.EqualTo(6));
    }

    [Test]
    public void TestCreateVar()
    {
        var (registers, variables) = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0),
            new IRInstruction(IROperation.CreateVar, 0, 0, 0)
        ], [
            IRConstant.From(42)
        ]);
        Assert.That(variables[0], Is.EqualTo(42));
        Assert.That(registers[0], Is.EqualTo(42));
    }

    [Test]
    public void TestGetVar()
    {
        var (registers, _) = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0),
            new IRInstruction(IROperation.CreateVar, 0, 0, 0),
            new IRInstruction(IROperation.GetVar, 0, 0, 1)
        ], [
            IRConstant.From(42)
        ]);
        Assert.That(registers[1], Is.EqualTo(42));
    }
}