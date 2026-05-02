using XMLSharpCompiler;

namespace XMLSharpCompilerTests;

[TestFixture]
[TestOf(typeof(AstGenerator))]
public class AstGeneratorTests
{
    [Test]
    public void TestCreateVariableAndAssignTwoPlusTwo()
    {
        Token[] tokenInput =
        [
            new VariableDefinitionToken(XMLSType.Number), new IdentifierToken("foo"), new AssignmentToken(),
            new ImmediateToken(2), new AddToken(), new ImmediateToken(2), new SemicolonToken()
        ];
        
        AstNode ast = new AstGenerator().Generate(tokenInput);

        AstNode expected = new CreateVariableNode
        (
            "foo",
            XMLSType.Number,
            new AddNode
            (
                new NumberNode(2),
                new NumberNode(2)
            )
        );
        
        Assert.That(ast, Is.EqualTo(expected));
    }

    [Test]
    public void TestExpressionWithVariableAndNumber()
    {
        Token[] tokenInput =
        [
            new ImmediateToken(2), new AddToken(), new IdentifierToken("bar"), new SemicolonToken()
        ];
        
        AstNode ast = new AstGenerator().Generate(tokenInput);
        
        AstNode expected = new AddNode
        (
            new NumberNode(2),
            new GetVariableNode("bar")
        );
        
        Assert.That(ast, Is.EqualTo(expected));
    }

    [Test]
    public void TestSetVariableToNumber()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new ImmediateToken(2), new SemicolonToken()
        ];
        
        AstNode ast = new AstGenerator().Generate(tokenInput);
        
        AstNode expected = new SetVariableNode("foo", new NumberNode(2));
        
        Assert.That(ast, Is.EqualTo(expected));   
    }
}