using System.Diagnostics;
using Common;

namespace XMLSharpCompiler;

public static class Compiler
{
    public static void Compile(CompilationSettings settings)
    {
        Console.WriteLine($"Compiling {settings.InputFile.FullName}...");
        
        Stopwatch sw = Stopwatch.StartNew();
        ILexer lexer = new Lexer();
        IAstGenerator astGenerator = new AstGenerator();
        SyntaxValidator validator = new();

        string fileContent = File.ReadAllText(settings.InputFile.FullName);
        var (tokens, lexErrors) = lexer.Lex(fileContent);

        if (settings.VerboseMode)
        {
            Console.WriteLine("Tokens:");
            tokens.PrettyPrint();
        }

        List<Diagnostic> errors = [..lexErrors];
        if (!settings.IgnoreErrors)
        {
            HashSet<(int Line, int Col)> lexPositions = [.. lexErrors.Select(e => (e.Line, e.Col))];
            Diagnostic[] validatorErrors = validator.Validate(tokens)
                .Where(e => !lexPositions.Contains((e.Line, e.Col)))
                .ToArray();
            errors.AddRange(validatorErrors);
        }

        if (errors.Count > 0)
        {
            ErrorReporter reporter = new();
            reporter.Report(fileContent, [..errors]);
            Environment.Exit(1);
        }
        
        IDesugarer desugarer = new Desugarer();
        Token[] desugaredTokens = desugarer.Desugar(tokens);
        
        if (settings.VerboseMode)
        {
            Console.WriteLine("Desugared Tokens:");
            desugaredTokens.PrettyPrint();
            
            Console.WriteLine("Desugared source:");
            Console.WriteLine(Detokeniser.ToSource(desugaredTokens));
        }
        
        AstNode ast = astGenerator.Generate(desugaredTokens);

        if (settings.VerboseMode)
        {
            Console.WriteLine("AST:");
            ast.PrettyPrint();
        }
        
        IIR irGenerator = new IR();
        IRProgram program = irGenerator.FromAst(ast);

        if (settings.VerboseMode)
        {
            Console.WriteLine("IR - Constants:");
            program.Constants.PrettyPrint();
            Console.WriteLine("IR - Instructions:");
            program.Instructions.PrettyPrint();
        }
        
        long compileTime = sw.ElapsedMilliseconds;
        
        Console.WriteLine("Compilation finished, writing IR to file...");
        
        irGenerator.WriteToFile(settings.OutputFile, program);
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Compilation completed in {compileTime}ms. Results saved to {settings.OutputFile.FullName} in {sw.ElapsedMilliseconds - compileTime}ms.");
        Console.ResetColor();
    }
}

public record CompilationSettings
{
    public required FileInfo InputFile;
    public required FileInfo OutputFile;
    
    public bool VerboseMode = false;
    public bool IgnoreErrors = false;
}
