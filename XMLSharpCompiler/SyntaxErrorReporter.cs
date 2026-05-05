using Common;

namespace XMLSharpCompiler;

public class ErrorReporter
{
    public void Report(string source, Diagnostic[] errors)
    {
        string[] lines = source.Replace("\r\n", "\n").Split('\n');


        foreach (Diagnostic error in errors)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Error.WriteLine($"{error.Type} at {error.Line}:{error.Col}");
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

        Console.BackgroundColor = ConsoleColor.DarkRed;
        string s = errors.Length == 1 ? "" : "s";
        Console.Error.WriteLine($" {errors.Length} error{s} found. Compilation aborted. ");
        Console.ResetColor();
    }
}