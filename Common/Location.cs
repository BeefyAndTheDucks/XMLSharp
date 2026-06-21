namespace Common;

public record Location(string? AbsoluteFilePath, int Line, int Column, int Length)
{
    public static Location From(FileInfo? file, int line, int column, int length) => new(file?.FullName, line, column, length);
    
    public override string ToString() => $"{AbsoluteFilePath ?? "<no file>"} ({Line},{Column})";
}
