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
        Lexer lexer = new();
        (Token[] tokens, Diagnostic[] errors) = lexer.Lex("number foo = 2;", null);
        Assert.That(errors, Is.Empty);
        Assert.That(tokens, Is.EqualTo(new Token[]
        {
            new TypeToken(XMLSType.Number, new Location(null, 1, 1, 6)),
            new IdentifierToken("foo", new Location(null, 1, 8, 3)),
            new AssignmentToken(new Location(null, 1, 12, 1)),
            new NumberToken(2, new Location(null, 1, 14, 1)),
            new SemicolonToken(new Location(null, 1, 15, 1)),
            new EOFToken(new Location(null, 1, 16, 0))
        }));
    }

    [Test]
    public void TestStrings()
    {
        Lexer lexer = new Lexer();
        (Token[] tokens, Diagnostic[] errors) = lexer.Lex("text foo = \"hello\";", null);
        Assert.That(errors, Is.Empty);
        Assert.That(tokens, Is.EqualTo(new Token[]
        {
            new TypeToken(XMLSType.Text, new Location(null, 1, 1, 4)),
            new IdentifierToken("foo", new Location(null, 1, 6, 3)),
            new AssignmentToken(new Location(null, 1, 10, 1)),
            new TextToken("hello", new Location(null, 1, 12, 7)),
            new SemicolonToken(new Location(null, 1, 19, 1)),
            new EOFToken(new Location(null, 1, 20, 0))
        }));
    }

    [Test]
    public void TestUnexpectedCharacter()
    {
        Lexer lexer = new();
        (_, Diagnostic[] errors) = lexer.Lex("number foo = @;", null);
        Assert.That(errors, Has.Length.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors[0].Location!.Line, Is.EqualTo(1));
            Assert.That(errors[0].Location!.Column, Is.EqualTo(14));
            Assert.That(errors[0].Message, Does.Contain("@"));
        }
    }
}