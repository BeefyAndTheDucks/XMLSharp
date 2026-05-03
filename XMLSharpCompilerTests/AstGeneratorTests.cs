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
            new NumberToken(2), new AddToken(), new NumberToken(2), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);

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
        
        Assert.That(ast, Is.EqualTo([expected]));
    }

    [Test]
    public void TestExpressionWithVariableAndNumber()
    {
        Token[] tokenInput =
        [
            new NumberToken(2), new AddToken(), new IdentifierToken("bar"), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        AstNode expected = new AddNode
        (
            new NumberNode(2),
            new GetVariableNode("bar")
        );
        
        Assert.That(ast, Is.EqualTo([expected]));
    }

    [Test]
    public void TestSetVariableToNumber()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(2), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        AstNode expected = new SetVariableNode("foo", new NumberNode(2));
        
        Assert.That(ast, Is.EqualTo([expected]));   
    }
    
    [Test]
    public void TestSetVariableToExpression()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(2), new AddToken(), new NumberToken(2),
            new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        AstNode expected = new SetVariableNode("foo", new AddNode(new NumberNode(2), new NumberNode(2)));
        
        Assert.That(ast, Is.EqualTo([expected]));  
    }
    
    [Test]
    public void TestSetVariableToExpressionWithVariable()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(2), new AddToken(),
            new IdentifierToken("bar"), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        AstNode expected = new SetVariableNode("foo", new AddNode(new NumberNode(2), new GetVariableNode("bar")));
        
        Assert.That(ast, Is.EqualTo([expected])); 
    }
    
    [Test]
    public void TestComplexExpression()
    {
        // foo = 1 + bar + 3;
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(1), new AddToken(),
            new IdentifierToken("bar"), new AddToken(), new NumberToken(3), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        // foo = 1 + (bar + 3);
        AstNode expected = new SetVariableNode("foo",
            new AddNode(new NumberNode(1), new AddNode(new GetVariableNode("bar"), new NumberNode(3))));
        
        Assert.That(ast, Is.EqualTo([expected]));
    }

    [Test]
    public void TestVeryComplexExpression()
    {
        // foo = 1 + bar + 2 + baz + 3
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(1), new AddToken(),
            new IdentifierToken("bar"), new AddToken(), new NumberToken(2), new AddToken(), new IdentifierToken("baz"),
            new AddToken(), new NumberToken(3), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        // foo = a + (bar + (2 + (baz + 3)))
        AstNode expected = new SetVariableNode("foo",
            new AddNode(new NumberNode(1), new AddNode(new GetVariableNode("bar"), new AddNode(new NumberNode(2), new AddNode(new GetVariableNode("baz"), new NumberNode(3))))));
        
        Assert.That(ast, Is.EqualTo([expected]));
    }
    
    [Test]
    public void TestGetVariable()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new GetVariableNode("foo")]));
    }

    [Test]
    public void TestNotOperator()
    {
        Token[] tokenInput =
        [
            new NotToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new NotNode(new BooleanNode(true))]));
    }

    [Test]
    public void TestAndOperator()
    {
        Token[] tokenInput =
        [
            new NoToken(), new AndToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new AndNode(new BooleanNode(false), new BooleanNode(true))]));
    }

    [Test]
    public void TestOrOperator()
    {
        Token[] tokenInput =
        [
            new NoToken(), new OrToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new OrNode(new BooleanNode(false), new BooleanNode(true))]));
    }

    [Test]
    public void TestXorOperator()
    {
        Token[] tokenInput =
        [
            new NoToken(), new XorToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new XorNode(new BooleanNode(false), new BooleanNode(true))]));
    }

    [Test]
    public void TestEqualsOperatorOnBooleans()
    {
        Token[] tokenInput =
        [
            new NoToken(), new EqualsToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new EqualNode(new BooleanNode(false), new BooleanNode(true))]));
    }

    [Test]
    public void TestEqualsOperatorOnNumbers()
    {
        Token[] tokenInput =
        [
            new NumberToken(5), new EqualsToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new EqualNode(new NumberNode(5), new NumberNode(7))]));
    }

    [Test]
    public void TestNotEqualsOperatorOnBooleans()
    {
        Token[] tokenInput =
        [
            new NoToken(), new NotEqualsToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new NotEqualNode(new BooleanNode(false), new BooleanNode(true))]));
    }

    [Test]
    public void TestNotEqualsOperatorOnNumbers()
    {
        Token[] tokenInput =
        [
            new NumberToken(5), new NotEqualsToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new NotEqualNode(new NumberNode(5), new NumberNode(7))]));
    }
    
    
    [Test]
    public void TestGreaterOperator()
    {
        Token[] tokenInput =
        [
            new NumberToken(5), new GreaterToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new GreaterThanNode(new NumberNode(5), new NumberNode(7))]));
    }
    
    [Test]
    public void TestLessOperator()
    {
        Token[] tokenInput =
        [
            new NumberToken(5), new LessToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new LessThanNode(new NumberNode(5), new NumberNode(7))]));
    }
    
    [Test]
    public void TestGreaterOrEqualOperator()
    {
        Token[] tokenInput =
        [
            new NumberToken(5), new GreaterOrEqualsToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new GreaterThanOrEqualNode(new NumberNode(5), new NumberNode(7))]));
    }
    
    [Test]
    public void TestLessOrEqualOperator()
    {
        Token[] tokenInput =
        [
            new NumberToken(5), new LessOrEqualsToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new LessThanOrEqualNode(new NumberNode(5), new NumberNode(7))]));
    }

    [Test]
    public void TestSubtract()
    {
        Token[] tokenInput =
        [
            new NumberToken(5), new SubtractToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new SubtractNode(new NumberNode(5), new NumberNode(7))]));
    }
    
    [Test]
    public void TestMultiply()
    {
        Token[] tokenInput =
        [
            new NumberToken(5), new MultiplyToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new MultiplyNode(new NumberNode(5), new NumberNode(7))]));
    }

    [Test]
    public void TestDivide()
    {
        Token[] tokenInput =
        [
            new NumberToken(5), new DivideToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new DivideNode(new NumberNode(5), new NumberNode(7))]));   
    }
    
    [Test]
    public void TestModulo()
    {
        Token[] tokenInput =
        [
            new NumberToken(5), new ModuloToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new ModuloNode(new NumberNode(5), new NumberNode(7))]));  
    }
    
    [Test]
    public void TestText()
    {
        Token[] tokenInput =
        [
            new TextToken("Hello, world!"), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new TextNode("Hello, world!")])); 
    }
    
    [Test]
    public void TestConcat()
    {
        Token[] tokenInput =
        [
            new TextToken("Hello, "), new ConcatToken(), new IdentifierToken("world"), new ConcatToken(), new TextToken("!"), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo([new ConcatNode(new TextNode("Hello, "), new ConcatNode(new GetVariableNode("world"), new TextNode("!")))]));
    }
}