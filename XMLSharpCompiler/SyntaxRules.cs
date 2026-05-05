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

    private void ValidateFor()
    {
        Advance(); // for
        Expect<OpenParenToken>("Expected '(' after 'for'.");
        Expect<TypeToken>("Expected type after 'for'.");
        Token nameTok = Current;
        Expect<IdentifierToken>("Expected identifier after type.");
        Expect<AssignmentToken>($"Expected '=' after {TokenName(nameTok)}.");
        ValidateExpression();
        Expect<SemicolonToken>("Expected ';' after initializer.");
        ValidateExpression();
        Expect<SemicolonToken>("Expected ';' after condition.");

        Token incTok = Current;
        Expect<IdentifierToken>("Expected variable name in for increment step.");

        if (Current is not IncrementToken and not DecrementToken
                    and not IncrementByToken and not DecrementByToken
                    and not MultiplyByToken and not DivideByToken and not ModuloByToken)
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

    private void ValidateExpression()
    {
        ValidateUnary();

        while (Current is AddToken or SubtractToken or MultiplyToken or DivideToken or ModuloToken
                        or GreaterToken or LessToken or GreaterOrEqualsToken or LessOrEqualsToken
                        or EqualsToken or NotEqualsToken or AndToken or OrToken or XorToken or ConcatToken)
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

    private void Synchronise()
    {
        while (Current is not EOFToken)
        {
            if (Current is SemicolonToken)
            {
                Advance();
                return;
            }
            if (Current is IfToken or WhileToken or ForToken or PrintToken
                or TypeToken or EndBlockToken
                or ElifToken or ElseToken)
                return;

            Advance();
        }
    }
}