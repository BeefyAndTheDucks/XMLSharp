using System.Diagnostics;
using Common;

namespace XMLSharpCompiler;

public static class Compiler
{
    public static void Compile(CompilationSettings settings)
    {
        Console.WriteLine($"Compiling {settings.InputFile.FullName}...");
        
        Directory.SetCurrentDirectory(settings.InputFile.DirectoryName!);
        
        var sw = Stopwatch.StartNew();
        ILexer lexer = new Lexer();
        ImportExpander importExpander = new(lexer);
        IAstGenerator astGenerator = new AstGenerator();
        SyntaxValidator validator = new();
        Desugarer desugarer = new();

        string fileContent = File.ReadAllText(settings.InputFile.FullName);
        var (tokens, errors) = lexer.Lex(fileContent, settings.InputFile)
            .AddProcessor(importExpander)
            .AddProcessor(validator)
            .AddProcessor(desugarer);

        if (errors.Length > 0)
        {
            ErrorReporter reporter = new();
            reporter.Report(errors);
            Environment.Exit(1);
        }
        
        if (settings.VerboseMode)
        {
            Console.WriteLine("Desugared Tokens:");
            tokens.PrettyPrint();
            
            Console.WriteLine("Desugared source:");
            Console.WriteLine(Detokeniser.ToSource(tokens));
        }
        
        AstNode ast = astGenerator.Generate(tokens);

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
