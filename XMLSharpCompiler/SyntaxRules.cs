using Common;

namespace XMLSharpCompiler;

public partial class SyntaxValidator
{
    private void ValidateStatement()
    {
        int posBeforeStatement = pos;

        switch (Current)
        {
            case ElifToken:
            case ElseToken:
            case EndBlockToken:
                Advance();
                return;
            case TypeToken: ValidateVariableDeclaration(); break;
            case IfToken: ValidateIf(); break;
            case WhileToken: ValidateWhile(); break;
            case ForToken: ValidateFor(); break;
            case PrintToken: ValidatePrint(); break;
            case IdentifierToken: ValidateAssignment(); break;
            case FunctionToken: ValidateFunctionDefinition(); break;
            case ReturnToken: ValidateReturn(); break;
            default:
                errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, $"Unexpected token {TokenName(Current)}.", Current.Line, Current.Col, Current.Length));
                Advance();
                return;
        }

        if (pos == posBeforeStatement)
            Synchronise();
    }

    private bool IsExpressionStart() => Current is NumberToken or DecimalToken or TextToken
        or YesToken or NoToken or IdentifierToken or OpenParenToken or NotToken;

    private void ValidateVariableDeclaration()
    {
        Advance(); // type
        Token nameTok = Current;
        Expect<IdentifierToken>("Expected identifier after type.");
        Expect<AssignmentToken>($"Expected '=' after {TokenName(nameTok)}.");
        if (!IsExpressionStart())
        {
            Token lastToken = pos > 0 ? tokens[pos - 1] : Current;
            errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, $"Expected expression after {TokenName(lastToken)}.", lastToken.Line, lastToken.Col, lastToken.Length));
            Synchronise();
            return;
        }
        ValidateExpression();
        Expect<SemicolonToken>("Expected ';' after statement.");
    }

    private void ValidateAssignment()
    {
        Token nameTok = Current;
        Advance(); // identifier

        switch (Current)
        {
            case AssignmentToken:
                Advance();
                ValidateExpression();
                Expect<SemicolonToken>("Expected ';' after statement.");
                break;
            case IncrementByToken:
            case DecrementByToken:
            case MultiplyByToken:
            case DivideByToken:
            case ModuloByToken:
                Advance();
                ValidateExpression();
                Expect<SemicolonToken>("Expected ';' after statement.");
                break;
            case IncrementToken:
            case DecrementToken:
                Advance();
                Expect<SemicolonToken>("Expected ';' after statement.");
                break;
            default:
                errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, $"Expected assignment operator after {TokenName(nameTok)}.", Current.Line, Current.Col, Current.Length));
                Synchronise();
                break;
        }
    }

    private void ValidatePrint()
    {
        Advance(); // write
        ValidateExpression();
        while (Check<ConcatToken>())
        {
            Advance();
            ValidateExpression();
        }
        Expect<SemicolonToken>("Expected ';' after statement.");
    }

    private void ValidateIf()
    {
        Advance(); // if
        Expect<OpenParenToken>("Expected '(' after 'if'.");
        ValidateExpression();
        Expect<CloseParenToken>("Expected ')' after condition.");
        ValidateBlock();

        while (Check<ElifToken>())
        {
            Advance();
            Expect<OpenParenToken>("Expected '(' after 'elif'.");
            ValidateExpression();
            Expect<CloseParenToken>("Expected ')' after condition.");
            ValidateBlock();
        }

        if (Match<ElseToken>())
            ValidateBlock();
    }

    private void ValidateWhile()
    {
        Advance(); // while
        Expect<OpenParenToken>("Expected '(' after 'while'.");
        ValidateExpression();
        Expect<CloseParenToken>("Expected ')' after condition.");
        ValidateBlock();
    }

    private static readonly HashSet<Type> ForIncrementOperators = new()
    {
        typeof(IncrementToken),
        typeof(DecrementToken),
        typeof(IncrementByToken),
        typeof(DecrementByToken),
        typeof(MultiplyByToken),
        typeof(DivideByToken),
        typeof(ModuloByToken),
    };

    private void ValidateFor()
    {
        Advance(); // for
        Expect<OpenParenToken>("Expected '(' after 'for'.");
        Token typeTok = Current;
        Expect<TypeToken>("Expected type after 'for'.");
        if (Current is not IdentifierToken)
        {
            errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, $"Expected identifier after {TokenName(typeTok)}.", typeTok.Line, typeTok.Col, typeTok.Length));
            Synchronise();
            return;
        }
        Token nameTok = Current;
        Expect<IdentifierToken>("Expected identifier after type.");
        Expect<AssignmentToken>($"Expected '=' after {TokenName(nameTok)}.");
        ValidateExpression();
        Expect<SemicolonToken>("Expected ';' after initializer.");
        ValidateExpression();
        Expect<SemicolonToken>("Expected ';' after condition.");

        Token incTok = Current;
        Expect<IdentifierToken>("Expected variable name in for increment step.");

        if (!ForIncrementOperators.Contains(Current.GetType()))
        {
            Token lastToken = pos > 0 ? tokens[pos - 1] : Current;
            errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, $"Expected increment or decrement after {TokenName(lastToken)}.", lastToken.Line, lastToken.Col, lastToken.Length));
            Synchronise();
            return;
        }

        switch (Current)
        {
            case IncrementToken:
            case DecrementToken:
                Advance();
                break;
            case IncrementByToken:
            case DecrementByToken:
            case MultiplyByToken:
            case DivideByToken:
            case ModuloByToken:
                Advance();
                ValidateExpression();
                break;
        }

        Expect<CloseParenToken>("Expected ')' after for increment.");
        ValidateBlock();
    }

    private void ValidateBlock()
    {
        Expect<BeginBlockToken>("Expected '{'.");
        while (Current is not EndBlockToken and not EOFToken)
            ValidateStatement();
        Expect<EndBlockToken>("Expected '}'.");
    }

    private static readonly HashSet<Type> ExpressionOperators = new()
    {
        typeof(AddToken),
        typeof(SubtractToken),
        typeof(MultiplyToken),
        typeof(DivideToken),
        typeof(ModuloToken),
        typeof(GreaterToken),
        typeof(LessToken),
        typeof(GreaterOrEqualsToken),
        typeof(LessOrEqualsToken),
        typeof(EqualsToken),
        typeof(NotEqualsToken),
        typeof(AndToken),
        typeof(OrToken),
        typeof(XorToken),
        typeof(ConcatToken),
    };

    private void ValidateExpression()
    {
        ValidateUnary();

        while (ExpressionOperators.Contains(Current.GetType()))
        {
            Advance();
            ValidateUnary();
        }
    }

    private void ValidateUnary()
    {
        if (Current is NotToken)
        {
            Advance();
            ValidatePrimary();
            return;
        }
        ValidatePrimary();
    }

    private void ValidatePrimary()
    {
        switch (Current)
        {
            case NumberToken:
            case DecimalToken:
            case TextToken:
            case YesToken:
            case NoToken:
            case IdentifierToken:
                Advance();
                if (Current is OpenParenToken) // Function call
                    ValidateFunctionCall();
                break;
            case OpenParenToken:
                Advance();
                ValidateExpression();
                Expect<CloseParenToken>("Expected ')' after expression.");
                break;
            case NotToken:
                Advance();
                ValidatePrimary();
                break;
            default:
                errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, $"Expected expression, got {TokenName(Current)}.", Current.Line, Current.Col, Current.Length));
                break;
        }
    }

    private void ValidateFunctionCall()
    {
        Advance();
        bool noComma = false;
        while (IsExpressionStart())
        {
            if (noComma)
            {
                errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Expected ',' between parameters.", Previous.Line, Previous.Col, Previous.Length));
                break;
            }
            ValidateExpression();
            if (Check<SeparatorToken>())
                Advance();
            else
                noComma = true;
        }
        Expect<CloseParenToken>("Expected ')' after parameters in function call.");
    }

    private void ValidateFunctionDefinition()
    {
        Advance(); // function
        if (Current is TypeToken)
            Advance();
        Expect<IdentifierToken>("Expected function name.");
        Expect<OpenParenToken>("Expected '(' after identifier in function definition.");
        bool noComma = false;
        while (Current is TypeToken)
        {
            if (noComma)
            {
                errors.Add(new Diagnostic(XMLSErrorType.SyntaxError, "Expected ',' between parameters.", Previous.Line, Previous.Col, Previous.Length));
                break;
            }
            // Handle parameters
            Advance();
            Expect<IdentifierToken>("Expected parameter name after parameter type in function definition.");
            if (Check<SeparatorToken>())
                Advance();
            else
                noComma = true;
        }
        Expect<CloseParenToken>("Expected ')' after parameters.");
        ValidateBlock();
    }

    private void ValidateReturn()
    {
        Advance(); // return
        ValidateExpression();
        Expect<SemicolonToken>("Expected ';' after return statement.");
    }

    // add any tokens that mark the start of a new statement or block boundary.
    private static readonly HashSet<Type> syncStopToken =
    [
        typeof(IfToken),
        typeof(WhileToken),
        typeof(ForToken),
        typeof(PrintToken),
        typeof(TypeToken),
        typeof(EndBlockToken),
        typeof(ElifToken),
        typeof(ElseToken),
        typeof(FunctionToken),
        typeof(ReturnToken),
    ];

    // use this to recover to the next safe token
    private void Synchronise()
    {
        while (Current is not EOFToken)
        {
            if (Current is SemicolonToken)
            {
                Advance();
                return;
            }
            if (syncStopToken.Contains(Current.GetType()))
                return;

            Advance();
        }
    }
}