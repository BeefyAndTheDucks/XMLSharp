using Common;

namespace XMLSharpCompiler;

public class ErrorReporter
{
    private readonly Dictionary<string, string[]> _sourcesCache = new();
    
    public void Report(Diagnostic[] errors)
    {
        foreach ((XMLSErrorType errorType, string errorMessage, Location? location) in errors)
        {
            Console.ForegroundColor = errorType switch
            {
                XMLSErrorType.SyntaxError => ConsoleColor.Red,
                XMLSErrorType.Warning => ConsoleColor.Yellow,

                _ => ConsoleColor.Cyan
            };
            
            string? relativeFilePath = GetRelativeFilePath(location?.AbsoluteFilePath);

            Console.Error.WriteLine(location is { Line: > 0, Column: > 0 }
                ? $"{errorType} at {location.Line}:{location.Column} in \"{relativeFilePath ?? "unknown file"}\""
                : $"{errorType} — {errorMessage}");

            Console.ResetColor();
            
            string? line = GetLine(location);

            if (line is not null)
            {
                string lineNum = location!.Line.ToString();
                string margin = new(' ', lineNum.Length);

                Console.Error.WriteLine($"{lineNum} | {line}");

                string padding = margin + " | " + new string(' ', Math.Max(0, location.Column - 1));
                string pointer = new('~', Math.Max(0, location.Length));

                Console.Error.WriteLine($"{padding}{pointer} {errorMessage}\n");
            }
        }

        Console.BackgroundColor = ConsoleColor.DarkRed;
        string s = errors.Length == 1 ? "" : "s";
        Console.Error.WriteLine($" {errors.Length} error{s} found. Compilation aborted. ");
        Console.ResetColor();
    }

    private string? GetLine(Location? location)
    {
        if (location is null) return null;
        string[]? lines = GetSource(location.AbsoluteFilePath);
        if (lines is null) return null;
        if (lines.Length <= location.Line) return null;
        return lines[location.Line - 1];
    }
    
    private string[]? GetSource(string? file)
    {
        return file is null 
            ? null 
            : GetSource(new FileInfo(file));
    }

    private string[]? GetSource(FileInfo? file)
    {
        if (file is null) return null;
        if (!file.Exists) return null;
        if (_sourcesCache.TryGetValue(file.FullName, out string[]? source))
            return source;
        string[] lines = File.ReadAllLines(file.FullName);
        _sourcesCache[file.FullName] = lines;
        return lines;
    }

    private string? GetRelativeFilePath(string? absolutePath)
    {
        if (absolutePath is null) return null;
        return Path.GetRelativePath(Directory.GetCurrentDirectory(), absolutePath);
    }
}
