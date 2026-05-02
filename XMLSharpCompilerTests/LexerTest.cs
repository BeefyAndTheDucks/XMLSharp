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
        Exception ex = Assert.Throws<Exception>(() => lexer.Lex("number foo = @;"));
        Assert.That(ex.Message, Does.Contain("1:14"));
    }
}