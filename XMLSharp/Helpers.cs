namespace XMLSharp;

public static class Helpers
{
    public static FileInfo GetIRFileForSourceFile(FileInfo sourceFile)
    {
        return new FileInfo(Path.Combine(sourceFile.Directory!.FullName,
            Path.GetFileNameWithoutExtension(sourceFile.Name) + ".ir"));
    }
}