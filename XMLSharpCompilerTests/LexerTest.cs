using Common;
using XMLSharpCompiler;

namespace XMLSharpCompilerTests;

[TestFixture]
[TestOf(typeof(Lexer))]
public class LexerTest
{
    [Test]
    public void TestVariables()
    {
        Lexer lexer = new Lexer();
        (Token[] tokens, Diagnostic[] errors) = lexer.Lex("number foo = 2;");
        Assert.That(errors, Is.Empty);
        Assert.That(tokens, Is.EqualTo(new Token[]
        {
            new TypeToken(XMLSType.Number, 1, 1, 6),
            new IdentifierToken("foo", 1, 8, 3),
            new AssignmentToken(1, 12, 1),
            new NumberToken(2, 1, 14, 1),
            new SemicolonToken(1, 15, 1),
            new EOFToken(1, 16, 0)
        }));
    }

    [Test]
    public void TestStrings()
    {
        Lexer lexer = new Lexer();
        (Token[] tokens, Diagnostic[] errors) = lexer.Lex("text foo = \"hello\";");
        Assert.That(errors, Is.Empty);
        Assert.That(tokens, Is.EqualTo(new Token[]
        {
            new TypeToken(XMLSType.Text, 1, 1, 4),
            new IdentifierToken("foo", 1, 6, 3),
            new AssignmentToken(1, 10, 1),
            new TextToken("hello", 1, 12, 7),
            new SemicolonToken(1, 19, 1),
            new EOFToken(1, 20, 0)
        }));
    }

    [Test]
    public void TestUnexpectedCharacter()
    {
        Lexer lexer = new Lexer();
        (Token[] tokens, Diagnostic[] errors) = lexer.Lex("number foo = @;");
        Assert.That(errors, Has.Length.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors[0].Line, Is.EqualTo(1));
            Assert.That(errors[0].Col, Is.EqualTo(14));
            Assert.That(errors[0].Message, Does.Contain("@"));
        }
    }
}