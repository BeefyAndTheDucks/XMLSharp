using Common;
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
            new TypeToken(XMLSType.Number), new IdentifierToken("foo"), new AssignmentToken(),
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
        
        Assert.That(ast, Is.EqualTo(expected));
    }

    [Test]
    public void TestExpressionWithVariableAndNumber()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(2), new AddToken(), new IdentifierToken("bar"), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        AstNode expected = new SetVariableNode
        (
            "foo",
            new AddNode(
                new NumberNode(2),
                new GetVariableNode("bar")
            )
        );
        
        Assert.That(ast, Is.EqualTo(expected));
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
        
        Assert.That(ast, Is.EqualTo(expected));   
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
        
        Assert.That(ast, Is.EqualTo(expected));  
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
        
        Assert.That(ast, Is.EqualTo(expected)); 
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
            new AddNode(new AddNode(new NumberNode(1), new GetVariableNode("bar")), new NumberNode(3)));
        
        Assert.That(ast, Is.EqualTo(expected));
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
        
        // foo = (((1 + bar) + 2) + baz) + 3
        AstNode expected = new SetVariableNode("foo",
            new AddNode(
                new AddNode(
                    new AddNode(
                        new AddNode(
                            new NumberNode(1),
                            new GetVariableNode("bar")
                        ),
                        new NumberNode(2)
                    ),
                    new GetVariableNode("baz")
                ),
                new NumberNode(3)
            )
        );
        
        Assert.That(ast, Is.EqualTo(expected));
    }
    
    [Test]
    public void TestGetVariable()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new IdentifierToken("bar"), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new GetVariableNode("bar"))));
    }

    [Test]
    public void TestNotOperator()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NotToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new NotNode(new BooleanNode(true)))));
    }

    [Test]
    public void TestAndOperator()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NoToken(), new AndToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new AndNode(new BooleanNode(false), new BooleanNode(true)))));
    }

    [Test]
    public void TestOrOperator()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NoToken(), new OrToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new OrNode(new BooleanNode(false), new BooleanNode(true)))));
    }

    [Test]
    public void TestXorOperator()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NoToken(), new XorToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new XorNode(new BooleanNode(false), new BooleanNode(true)))));
    }

    [Test]
    public void TestEqualsOperatorOnBooleans()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NoToken(), new EqualsToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new EqualNode(new BooleanNode(false), new BooleanNode(true)))));
    }

    [Test]
    public void TestEqualsOperatorOnNumbers()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(5), new EqualsToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new EqualNode(new NumberNode(5), new NumberNode(7)))));
    }

    [Test]
    public void TestNotEqualsOperatorOnBooleans()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NoToken(), new NotEqualsToken(), new YesToken(), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new NotEqualNode(new BooleanNode(false), new BooleanNode(true)))));
    }

    [Test]
    public void TestNotEqualsOperatorOnNumbers()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(5), new NotEqualsToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new NotEqualNode(new NumberNode(5), new NumberNode(7)))));
    }
    
    
    [Test]
    public void TestGreaterOperator()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(5), new GreaterToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new GreaterThanNode(new NumberNode(5), new NumberNode(7)))));
    }
    
    [Test]
    public void TestLessOperator()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(5), new LessToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new LessThanNode(new NumberNode(5), new NumberNode(7)))));
    }
    
    [Test]
    public void TestGreaterOrEqualOperator()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(5), new GreaterOrEqualsToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new GreaterThanOrEqualNode(new NumberNode(5), new NumberNode(7)))));
    }
    
    [Test]
    public void TestLessOrEqualOperator()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(5), new LessOrEqualsToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];

        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new LessThanOrEqualNode(new NumberNode(5), new NumberNode(7)))));
    }

    [Test]
    public void TestSubtract()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(5), new SubtractToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new SubtractNode(new NumberNode(5), new NumberNode(7)))));
    }
    
    [Test]
    public void TestMultiply()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(5), new MultiplyToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new MultiplyNode(new NumberNode(5), new NumberNode(7)))));
    }

    [Test]
    public void TestDivide()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(5), new DivideToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new DivideNode(new NumberNode(5), new NumberNode(7)))));   
    }
    
    [Test]
    public void TestModulo()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new NumberToken(5), new ModuloToken(), new NumberToken(7), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new ModuloNode(new NumberNode(5), new NumberNode(7)))));  
    }
    
    [Test]
    public void TestText()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new TextToken("Hello, world!"), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new TextNode("Hello, world!")))); 
    }
    
    [Test]
    public void TestConcat()
    {
        Token[] tokenInput =
        [
            new IdentifierToken("foo"), new AssignmentToken(), new TextToken("Hello, "), new ConcatToken(), new IdentifierToken("world"), new ConcatToken(), new TextToken("!"), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new SetVariableNode("foo", new ConcatNode(new ConcatNode(new TextNode("Hello, "), new GetVariableNode("world")), new TextNode("!")))));
    }

    [Test]
    public void TestParseExpressionWithSamePrecedence()
    {
        Token[] tokenInput =
        [
            new NumberToken(1), new AddToken(), new NumberToken(2), new AddToken(), new NumberToken(3), new SemicolonToken(), new EOFToken()
        ];
        
        AstNode ast = new AstGenerator().ParseExpression(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new AddNode(new AddNode(new NumberNode(1), new NumberNode(2)), new NumberNode(3))));
    }
    
    [Test]
    public void TestParseExpressionWithHigherPrecedence()
    {
        Token[] tokenInput =
        [
            new NumberToken(1), new AddToken(), new NumberToken(2), new MultiplyToken(), new NumberToken(3), new SemicolonToken(), new EOFToken()
        ];
        
        AstNode ast = new AstGenerator().ParseExpression(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new AddNode(new NumberNode(1), new MultiplyNode(new NumberNode(2), new NumberNode(3)))));
    }

    [Test]
    public void TestParseExpressionWithParentheses()
    {
        // 1 * (2 + 3) * 4;
        Token[] tokenInput =
        [
            new NumberToken(1), new MultiplyToken(), new OpenParenToken(), new NumberToken(2), new AddToken(), new NumberToken(3), new CloseParenToken(), new MultiplyToken(), new NumberToken(4), new SemicolonToken(), new EOFToken()
        ];
        
        AstNode ast = new AstGenerator().ParseExpression(tokenInput);

        Assert.That(ast, Is.EqualTo(
            new MultiplyNode(
                new MultiplyNode(
                    new NumberNode(1),
                    new AddNode(
                        new NumberNode(2),
                        new NumberNode(3)
                    )
                ),
                new NumberNode(4)
            )
        ));
    }

    [Test]
    public void TestPrintFunction()
    {
        Token[] tokenInput =
        [
            new PrintToken(), new NumberToken(1), new SemicolonToken(), new EOFToken()
        ];
        
        var ast = new AstGenerator().Generate(tokenInput);
        
        Assert.That(ast, Is.EqualTo(new PrintNode(new NumberNode(1))));
    }

    [Test]
    public void TestFunctionWithExpression()
    {
        Token[] tokenInput =
        [
            new NumberToken(1), new AddToken(), new IdentifierToken("foo"), new OpenParenToken(), new NumberToken(1), new AddToken(), new NumberToken(1), new CloseParenToken(), new SemicolonToken(), new EOFToken()
        ];

        AstNode expected = new AddNode(
            new NumberNode(1),
            new CallFunctionNode("foo", [new AddNode(new NumberNode(1), new NumberNode(1))])
        );
        
        var ast = new AstGenerator().ParseExpression(tokenInput);
        
        Assert.That(ast, Is.EqualTo(expected));
    }
}