using System.Reflection;

namespace XMLSharpCompiler;

public static class Version
{
    public static string GetExecutableVersion()
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        if (assemblyVersionAttribute is null)
        {
            return assembly.GetName().Version?.ToString() ?? "";
        }
        else
        {
            return assemblyVersionAttribute.InformationalVersion;
        }
    }
}