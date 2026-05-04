namespace XMLSharpCompiler;

public static class TokenFollowers
{
    // token groups for your sanity

    // Anything
    private static readonly Type[] AfterAnything = [typeof(CloseParenToken), typeof(EndBlockToken)];

    // numbers
    private static readonly Type[] Numeric = [typeof(NumberToken), typeof(DecimalToken)];

    // truthiness
    private static readonly Type[] Boolean = [typeof(YesToken), typeof(NoToken)];

    // value in expressions
    private static readonly Type[] AllValues =
    [
        ..AfterAnything,
        ..Numeric,
        ..Boolean,
        typeof(TextToken),
        typeof(IdentifierToken),
        typeof(OpenParenToken),
    ];

    // maths operators
    private static readonly Type[] MathOps =
    [
        ..AfterAnything,
        typeof(AddToken),
        typeof(SubtractToken),
        typeof(MultiplyToken),
        typeof(DivideToken),
        typeof(ModuloToken),
        typeof(IncrementToken),
        typeof(DecrementToken),
        typeof(IncrementByToken),
        typeof(DecrementByToken),
        typeof(MultiplyByToken),
        typeof(DivideByToken),
        typeof(ModuloByToken),
    ];

    // comparison operators
    private static readonly Type[] ComparisonOps =
    [
        ..AfterAnything,
        typeof(EqualsToken),
        typeof(NotEqualsToken),
        typeof(GreaterToken),
        typeof(LessToken),
        typeof(GreaterOrEqualsToken),
        typeof(LessOrEqualsToken)
    ];

    // logical operators
    private static readonly Type[] LogicalOps = [typeof(AndToken), typeof(OrToken), typeof(XorToken), .. AfterAnything];

    // valid followers map
    public static readonly Dictionary<Type, HashSet<Type>> ValidFollowers = new()
    {
        // valid followers for values.
        [typeof(NumberToken)] = [typeof(SemicolonToken), .. MathOps, .. ComparisonOps, .. AfterAnything],
        [typeof(DecimalToken)] = [typeof(SemicolonToken), .. MathOps, .. ComparisonOps, .. AfterAnything],
        [typeof(TextToken)] = [typeof(SemicolonToken), typeof(ConcatToken), typeof(EqualsToken), typeof(NotEqualsToken), .. AfterAnything],

        [typeof(IdentifierToken)] =
        [
            typeof(SemicolonToken),
            typeof(AssignmentToken),
            ..MathOps,
            ..ComparisonOps,
            ..LogicalOps,
            typeof(ConcatToken),
            ..AfterAnything
        ],

        [typeof(YesToken)] = [typeof(SemicolonToken), .. LogicalOps, typeof(EqualsToken), typeof(NotEqualsToken), .. AfterAnything],
        [typeof(NoToken)] = [typeof(SemicolonToken), .. LogicalOps, typeof(EqualsToken), typeof(NotEqualsToken), .. AfterAnything],

        // valid followers for operators
        [typeof(AddToken)] = [.. Numeric, typeof(IdentifierToken), typeof(OpenParenToken)],
        [typeof(SubtractToken)] = [.. Numeric, typeof(IdentifierToken), typeof(OpenParenToken)],
        [typeof(MultiplyToken)] = [.. Numeric, typeof(IdentifierToken), typeof(OpenParenToken)],
        [typeof(DivideToken)] = [.. Numeric, typeof(IdentifierToken), typeof(OpenParenToken)],
        [typeof(ModuloToken)] = [.. Numeric, typeof(IdentifierToken), typeof(OpenParenToken)],

        [typeof(ConcatToken)] = [typeof(TextToken), typeof(IdentifierToken), typeof(OpenParenToken)],

        [typeof(AssignmentToken)] = [.. AllValues, typeof(NotToken)],
        [typeof(EqualsToken)] = [.. AllValues, typeof(NotToken)],
        [typeof(NotEqualsToken)] = [.. AllValues, typeof(NotToken)],

        [typeof(GreaterToken)] = [.. Numeric, typeof(IdentifierToken), typeof(OpenParenToken)],
        [typeof(LessToken)] = [.. Numeric, typeof(IdentifierToken), typeof(OpenParenToken)],
        [typeof(GreaterOrEqualsToken)] = [.. Numeric, typeof(IdentifierToken), typeof(OpenParenToken)],
        [typeof(LessOrEqualsToken)] = [.. Numeric, typeof(IdentifierToken), typeof(OpenParenToken)],

        [typeof(AndToken)] = [.. Boolean, typeof(IdentifierToken), typeof(OpenParenToken), typeof(NotToken)],
        [typeof(OrToken)] = [.. Boolean, typeof(IdentifierToken), typeof(OpenParenToken), typeof(NotToken)],
        [typeof(XorToken)] = [.. Boolean, typeof(IdentifierToken), typeof(OpenParenToken), typeof(NotToken)],

        [typeof(NotToken)] = [.. Boolean, typeof(IdentifierToken), typeof(OpenParenToken)],

        // parens
        [typeof(OpenParenToken)] = [.. AllValues, typeof(NotToken), typeof(OpenParenToken), typeof(CloseParenToken)],
        [typeof(CloseParenToken)] =
        [
            typeof(SemicolonToken),
            ..MathOps,
            ..ComparisonOps,
            ..LogicalOps,
            typeof(CloseParenToken),
            typeof(BeginBlockToken)
        ],

        // keywords
        [typeof(PrintToken)] = [.. AllValues, typeof(NotToken)],

        [typeof(VariableDefinitionToken)] = [typeof(IdentifierToken)],
    };
}