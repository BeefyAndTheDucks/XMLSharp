using Common;
using XMLSharpInterpreter;

namespace XMLSharpInterpreterTests;



[TestFixture]
[TestOf(typeof(Interpreter))]
public class InterpreterTest
{
    private static (Dictionary<int, dynamic> Registers, Dictionary<int, dynamic> Variables) Run(IRInstruction[] instructions)
    {
        Interpreter interpreter = new();
        interpreter.Run(instructions, true);
        return (interpreter.Registers, interpreter.Variables);
    }

    [Test]
    public void TestConstant()
    {
        var (registers, _) = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 42)
        ]);
        Assert.That(registers[0], Is.EqualTo(42));
    }

    [Test]
    public void TestAdd()
    {
        // t0 = 5, t1 = 3, t2 = t0 + t1
        var (registers, _) = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 5),
            new IRInstruction(IROperation.Constant, 0, 0, 1, 3),
            new IRInstruction(IROperation.Add,      0, 1, 2)
        ]);
        Assert.That(registers[2], Is.EqualTo(8));
    }

    [Test]
    public void TestSub()
    {
        var (registers, _) = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 10),
            new IRInstruction(IROperation.Constant, 0, 0, 1, 4),
            new IRInstruction(IROperation.Sub,      0, 1, 2)
        ]);
        Assert.That(registers[2], Is.EqualTo(6));
    }

    [Test]
    public void TestCreateVar()
    {
        var (registers, variables) = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 42),
            new IRInstruction(IROperation.CreateVar, 0, 0, 0)
        ]);
        Assert.That(variables[0], Is.EqualTo(42));
        Assert.That(registers[0], Is.EqualTo(42));
    }

    [Test]
    public void TestGetVar()
    {
        var (registers, _) = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 42),
            new IRInstruction(IROperation.CreateVar, 0, 0, 0),
            new IRInstruction(IROperation.GetVar, 0, 0, 1)
        ]);
        Assert.That(registers[1], Is.EqualTo(42));
    }
}