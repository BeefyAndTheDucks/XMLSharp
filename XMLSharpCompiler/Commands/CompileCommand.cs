using Common;

using System.CommandLine;
using System.Diagnostics;

namespace XMLSharpCompiler.Commands;

public class CompileCommand : CommandBase
{
    private readonly Argument<FileInfo> _fileArg = new("file");
    private readonly Option<FileInfo> _outputFileArg = new("output");
    
    private readonly Option<bool> _verboseArg = new("--verbose", "-verbose", "-v", "--v");
    private readonly Option<bool> _ignoreErrorsArg = new("--ignore-errors", "-i", "-ignore-errs", "-ierr");

    protected override void Invoke(ParseResult parseResult)
    {
        FileInfo inputFile = parseResult.GetRequiredValue(_fileArg);
        if (!inputFile.Exists)
        {
            Console.Error.WriteLine($"File {inputFile.FullName} does not exist.");
            Environment.Exit(1);
        }
        
        FileInfo outputFile = parseResult.GetValue(_outputFileArg) ?? new FileInfo(
            Path.Combine(inputFile.Directory!.FullName, Path.GetFileNameWithoutExtension(inputFile.Name) + ".ir"));

        bool verbose = parseResult.GetValue(_verboseArg);

        Console.WriteLine($"Compiling {inputFile.FullName}...");
        
        Stopwatch sw = Stopwatch.StartNew();
        ILexer lexer = new Lexer();
        IAstGenerator astGenerator = new AstGenerator();
        SyntaxValidator validator = new();

        string fileContent = File.ReadAllText(inputFile.FullName);
        Token[] tokens = lexer.Lex(fileContent);

        if (verbose)
        {
            Console.WriteLine("Tokens:");
            tokens.PrettyPrint();
        }

        Diagnostic[] errors = [];
        if (!parseResult.GetValue(_ignoreErrorsArg))
            errors = validator.Validate(tokens);

        if (errors.Length > 0)
        {
            ErrorReporter reporter = new();
            reporter.Report(fileContent, errors);
            Environment.Exit(1);
        }
        
        IDesugarer desugarer = new Desugarer();
        Token[] desugaredTokens = desugarer.Desugar(tokens);
        
        if (verbose)
        {
            Console.WriteLine("Desugared Tokens:");
            desugaredTokens.PrettyPrint();
            
            Console.WriteLine("Desugared source:");
            Console.WriteLine(Detokeniser.ToSource(desugaredTokens));
        }
        
        AstNode ast = astGenerator.Generate(desugaredTokens);

        if (verbose)
        {
            Console.WriteLine("AST:");
            ast.PrettyPrint();
        }
        
        IIR irGenerator = new IR();
        IRInstruction[] instructions = irGenerator.FromAst(ast);

        if (verbose)
        {
            Console.WriteLine("IR:");
            instructions.PrettyPrint();
        }
        
        long compileTime = sw.ElapsedMilliseconds;
        
        Console.WriteLine("Compilation finished, writing IR to file...");
        
        irGenerator.WriteToFile(outputFile, instructions);
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Compilation completed in {compileTime}ms. Results saved to {outputFile.FullName} in {sw.ElapsedMilliseconds - compileTime}ms.");
        Console.ResetColor();
    }

    protected override Command GetCommand()
    {
        return new Command("compile", "Compile a XMLSharp file to IR which can be executed by the interpreter")
        {
            _fileArg,
            _outputFileArg,
            _verboseArg,
            _ignoreErrorsArg
        };
    }
}