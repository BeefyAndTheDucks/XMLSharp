using XMLSharpCompiler;

[TestFixture]
[TestOf(typeof(Lexer))]
public class LexerTest
{
    [Test]
    public void TestVariables()
    {
        Lexer lexer = new Lexer();
        Token[] tokens = lexer.Lex("number foo = 2;");
        Assert.That(tokens, Is.EqualTo(new Token[]
        {
            new VariableDefinitionToken(XMLSType.Number),
            new IdentifierToken("foo"),
            new AssignmentToken(),
            new NumberToken(2),
            new SemicolonToken(),
            new EOFToken()
        }));
    }

    [Test]
    public void TestStrings()
    {
        Lexer lexer = new Lexer();
        Token[] tokens = lexer.Lex("text foo = \"hello\";");
        Assert.That(tokens, Is.EqualTo(new Token[]
        {
        new VariableDefinitionToken(XMLSType.Text),
        new IdentifierToken("foo"),
        new AssignmentToken(),
        new TextToken("hello"),
        new SemicolonToken(),
        new EOFToken()
        }));
    }

    [Test]
    public void TestUnexpectedCharacter()
    {
        Lexer lexer = new Lexer();
        UnexpectedCharacterException ex = Assert.Throws<UnexpectedCharacterException>(() => lexer.Lex("number foo = @;"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex.Line, Is.EqualTo(1));
            Assert.That(ex.Col, Is.EqualTo(14));
            Assert.That(ex.Character, Is.EqualTo('@'));
        }
    }
}