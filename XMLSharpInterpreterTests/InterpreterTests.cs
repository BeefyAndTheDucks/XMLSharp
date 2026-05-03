using Common;
using XMLSharpInterpreter;

namespace XMLSharpInterpreterTests;



[TestFixture]
[TestOf(typeof(Interpreter))]
public class InterpreterTest
{
    private Dictionary<int, object> Run(IRInstruction[] instructions)
    {
        Interpreter interpreter = new();
        interpreter.Run(instructions);
        return interpreter.Registers;
    }

    private Interpreter interpreter;

    [SetUp]
    public void SetUp()
    {
        interpreter = new Interpreter();
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
            new IRInstruction(IROperation.Add,      0, 1, 2, null)
        ]);
        Assert.That(registers[2], Is.EqualTo(8));
    }

    [Test]
    public void TestSub()
    {
        Dictionary<int, object> registers = Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 10),
            new IRInstruction(IROperation.Constant, 0, 0, 1, 4),
            new IRInstruction(IROperation.Sub,      0, 1, 2, null)
        ]);
        Assert.That(registers[2], Is.EqualTo(6));
    }

    [Test]
    public void TestCreateVar()
    {
        interpreter.Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 42),
            new IRInstruction(IROperation.CreateVar, 0, 0, 0, "foo")
        ]);
        Assert.That(interpreter.SymbolTable["foo"], Is.EqualTo(0));
        Assert.That(interpreter.Registers[0], Is.EqualTo(42));
    }

    [Test]
    public void TestGetVar()
    {
        interpreter.Run([
            new IRInstruction(IROperation.Constant, 0, 0, 0, 42),
            new IRInstruction(IROperation.CreateVar, 0, 0, 0, "foo"),
            new IRInstruction(IROperation.GetVar, 0, 0, 1, "foo")
        ]);
        Assert.That(interpreter.Registers[1], Is.EqualTo(42));
    }
}