using Common;
using XMLSharpCompiler;

namespace XMLSharpCompilerTests;

[TestFixture]
[TestOf(typeof(SyntaxValidator))]
public class SyntaxValidatorTest
{
    private readonly SyntaxValidator _validator = new();

    [Test]
    public void TestValidVariableDeclaration()
    {
        Token[] tokens = [
            new TypeToken(XMLSType.Number, new Location(null, 1, 1, 0)),
            new IdentifierToken("foo", new Location(null, 1, 8, 0)),
            new AssignmentToken(new Location(null, 1, 12, 0)),
            new NumberToken(10, new Location(null, 1, 14, 0)),
            new SemicolonToken(new Location(null, 1, 16, 0)),
            new EOFToken()
        ];

        var (_, errors) = _validator.Process(tokens, []);
        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void TestMissingIdentifierAfterType()
    {
        Token[] tokens = [
            new TypeToken(XMLSType.Number, new Location(null, 1, 1, 0)),
            new AssignmentToken(new Location(null, 1, 8, 0)),
            new EOFToken()
        ];

        var (_, errors) = _validator.Process(tokens, []);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Length.EqualTo(2));
            Assert.That(errors[0].Message, Is.EqualTo("Expected identifier after type."));
            Assert.That(errors[0].Location!.Line, Is.EqualTo(1));
            Assert.That(errors[0].Location!.Column, Is.EqualTo(1));
            Assert.That(errors[1].Message, Is.EqualTo("Expected expression after '='."));
            Assert.That(errors[1].Location!.Line, Is.EqualTo(1));
            Assert.That(errors[1].Location!.Column, Is.EqualTo(8));
        }
    }

    [Test]
    public void TestMissingAssignmentAfterIdentifier()
    {
        Token[] tokens = [
            new TypeToken(XMLSType.Number, new Location(null, 1, 1, 6)),
            new IdentifierToken("foo", new Location(null, 1, 8, 3)),
            new NumberToken(10, new Location(null, 1, 12, 2)),
            new SemicolonToken(new Location(null, 1, 14, 1)),
            new EOFToken()
        ];

        var (_, errors) = _validator.Process(tokens, []);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Length.EqualTo(1));
            Assert.That(errors[0].Message, Is.EqualTo("Expected '=' after 'foo'."));
            Assert.That(errors[0].Location!.Line, Is.EqualTo(1));
            Assert.That(errors[0].Location!.Column, Is.EqualTo(8));
        }
    }
    
    [Test]
    public void TestPrintFunction()
    {
        Token[] tokens = [
            new PrintToken(new Location(null, 1, 1, 0)),
            new TextToken("foo", new Location(null, 1, 7, 0)),
            new ConcatToken(new Location(null, 1, 11, 0)),
            new TextToken("bar", new Location(null, 1, 13, 0)),
            new SemicolonToken(new Location(null, 1, 16, 0)),
            new EOFToken()
        ];
        
        var (_, errors) = _validator.Process(tokens, []);
        Assert.That(errors, Is.Empty);
    }
}