namespace XMLSharpCompiler;

public class ErrorReporter
{
    public void Report(string source, SyntaxError[] errors)
    {
        string[] lines = source.Replace("\r\n", "\n").Split('\n');

        ConsoleColor red = ConsoleColor.Red;
        ConsoleColor darkRed = ConsoleColor.DarkRed;

        foreach (SyntaxError error in errors)
        {
            Console.ForegroundColor = red;
            Console.Error.WriteLine($"Syntax error at {error.Line}:{error.Col}");
            Console.ResetColor();

            if (error.Line > 0 && error.Line <= lines.Length)
            {
                string lineText = lines[error.Line - 1];
                string lineNum = error.Line.ToString();
                string margin = new(' ', lineNum.Length);

                Console.Error.WriteLine($"{lineNum} | {lineText}");

                string padding = margin + " | " + new string(' ', Math.Max(0, error.Col - 1));
                string pointer = new('~', Math.Max(0, error.Length));

                Console.Error.WriteLine($"{padding}{pointer} {error.Message}\n");
            }
        }

        Console.BackgroundColor = darkRed;
        string s = errors.Length == 1 ? "" : "s";
        Console.Error.WriteLine($" {errors.Length} syntax error{s} found. Compilation aborted. ");
        Console.ResetColor();
    }
}