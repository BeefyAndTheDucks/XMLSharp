using Common;
using XMLSharpInterpreter;

namespace XMLSharpInterpreterTests;



[TestFixture]
[TestOf(typeof(Interpreter))]
public class InterpreterTest
{
    private static Dictionary<int, object> Run(IRInstruction[] instructions)
    {
        Interpreter interpreter = new();
        interpreter.Run(instructions);
        return interpreter.Registers;
    }

    private Interpreter _interpreter;

    [SetUp]
    public void SetUp()
    {
        _interpreter = new Interpreter();
    }

    [Test]
    public void TestConstant()
    {
        Dictionary<int, object> registers = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 42)
        ]);
        Assert.That(registers[0], Is.EqualTo(42));
    }

    [Test]
    public void TestAdd()
    {
        // t0 = 5, t1 = 3, t2 = t0 + t1
        Dictionary<int, object> registers = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 5),
            new IRInstruction(IROperation.Constant, 0, 0, 1, 3),
            new IRInstruction(IROperation.Add,      0, 1, 2)
        ]);
        Assert.That(registers[2], Is.EqualTo(8));
    }

    [Test]
    public void TestSub()
    {
        Dictionary<int, object> registers = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 10),
            new IRInstruction(IROperation.Constant, 0, 0, 1, 4),
            new IRInstruction(IROperation.Sub,      0, 1, 2)
        ]);
        Assert.That(registers[2], Is.EqualTo(6));
    }

    [Test]
    public void TestCreateVar()
    {
        _interpreter.Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 42),
            new IRInstruction(IROperation.CreateVar, 0, 0, 0)
        ]);
        Assert.That(_interpreter.Variables[0], Is.EqualTo(42));
        Assert.That(_interpreter.Registers[0], Is.EqualTo(42));
    }

    [Test]
    public void TestGetVar()
    {
        _interpreter.Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 42),
            new IRInstruction(IROperation.CreateVar, 0, 0, 0),
            new IRInstruction(IROperation.GetVar, 0, 0, 1)
        ]);
        Assert.That(_interpreter.Registers[1], Is.EqualTo(42));
    }
}