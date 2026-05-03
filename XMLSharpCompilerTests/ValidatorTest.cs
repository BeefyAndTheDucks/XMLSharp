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
            new VariableDefinitionToken(XMLSType.Number, 1, 1),
            new IdentifierToken("foo", 1, 8),
            new AssignmentToken(1, 12),
            new NumberToken(10, 1, 14),
            new SemicolonToken(1, 16),
            new EOFToken()
        ];

        SyntaxError[] errors = _validator.Validate(tokens);
        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void TestMissingIdentifierAfterType()
    {
        Token[] tokens = [
            new VariableDefinitionToken(XMLSType.Number, 1, 1),
            new AssignmentToken(1, 8),
            new EOFToken()
        ];

        SyntaxError[] errors = _validator.Validate(tokens);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Length.EqualTo(1));
            Assert.That(errors[0].Message, Is.EqualTo("Expected identifier after type."));
            Assert.That(errors[0].Line, Is.EqualTo(1));
            Assert.That(errors[0].Col, Is.EqualTo(1));
        }
    }

    [Test]
    public void TestMissingAssignmentAfterIdentifier()
    {
        Token[] tokens = [
            new VariableDefinitionToken(XMLSType.Number, 1, 1),
        new IdentifierToken("foo", 1, 8),
        new NumberToken(10, 1, 12),
        new EOFToken()
        ];

        SyntaxError[] errors = _validator.Validate(tokens);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Length.EqualTo(2));
            Assert.That(errors[0].Message, Is.EqualTo("Expected '=' after 'foo'."));
            Assert.That(errors[0].Line, Is.EqualTo(1));
            Assert.That(errors[0].Col, Is.EqualTo(8));
            Assert.That(errors[1].Message, Is.EqualTo("Unexpected token 'NumberToken'."));
            Assert.That(errors[1].Line, Is.EqualTo(1));
            Assert.That(errors[1].Col, Is.EqualTo(12));
        }
    }
}