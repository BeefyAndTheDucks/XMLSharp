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
            new NumberToken(2), new AddToken(), new NumberToken(2), new SemicolonToken()
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
            new NumberToken(2), new AddToken(), new IdentifierToken("bar"), new SemicolonToken()
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
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(2), new SemicolonToken()
        ];
        
        AstNode ast = new AstGenerator().Generate(tokenInput);
        
        AstNode expected = new SetVariableNode("foo", new NumberNode(2));
        
        Assert.That(ast, Is.EqualTo(expected));   
    }
    
    [Test]
    public void TestSetVariableToExpression()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new ImmediateToken(2), new AddToken(), new ImmediateToken(2), new SemicolonToken()
        ];
        
        AstNode ast = new AstGenerator().Generate(tokenInput);
        
        AstNode expected = new SetVariableNode("foo", new AddNode(new NumberNode(2), new NumberNode(2)));
        
        Assert.That(ast, Is.EqualTo(expected));  
    }
    
    [Test]
    public void TestSetVariableToExpressionWithVariable()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new ImmediateToken(2), new AddToken(), new IdentifierToken("bar"), new SemicolonToken()
        ];
        
        AstNode ast = new AstGenerator().Generate(tokenInput);
        
        AstNode expected = new SetVariableNode("foo", new AddNode(new NumberNode(2), new GetVariableNode("bar")));
        
        Assert.That(ast, Is.EqualTo(expected)); 
    }
    
    [Test]
    public void TestComplexExpression()
    {
        // foo = 1 + bar + 3;
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new ImmediateToken(1), new AddToken(), new IdentifierToken("bar"), new AddToken(), new ImmediateToken(3), new SemicolonToken()
        ];
        
        AstNode ast = new AstGenerator().Generate(tokenInput);
        
        // foo = 1 + (bar + 3);
        AstNode expected = new SetVariableNode("foo",
            new AddNode(new NumberNode(1), new AddNode(new GetVariableNode("bar"), new NumberNode(3))));
        
        Assert.That(ast, Is.EqualTo(expected));
    }

    [Test]
    public void TestVeryComplexExpression()
    {
        // foo = 1 + bar + 2 + baz + 3
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new ImmediateToken(1), new AddToken(), new IdentifierToken("bar"), new AddToken(), new ImmediateToken(2), new AddToken(), new IdentifierToken("baz"), new AddToken(), new ImmediateToken(3), new SemicolonToken()
        ];
        
        AstNode ast = new AstGenerator().Generate(tokenInput);
        
        // foo = a + (bar + (2 + (baz + 3)))
        AstNode expected = new SetVariableNode("foo",
            new AddNode(new NumberNode(1), new AddNode(new GetVariableNode("bar"), new AddNode(new NumberNode(2), new AddNode(new GetVariableNode("baz"), new NumberNode(3))))));
        
        Assert.That(ast, Is.EqualTo(expected));
    }
    
    [Test]
    public void TestGetVariable()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new SemicolonToken()
        ];
        
        AstNode ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new GetVariableNode("foo")));
    }
}