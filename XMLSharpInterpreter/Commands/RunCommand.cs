using System.CommandLine;
using Common;

namespace XMLSharpInterpreter.Commands;

public class RunCommand : CommandBase
{
    private readonly Argument<FileInfo> _fileArg = new("file");
    
    protected override void Invoke(ParseResult parseResult)
    {
        throw new NotImplementedException();
    }

    protected override Command GetCommand()
    {
        return new Command("run", "Run a compiled XMLSharp file")
        {
            _fileArg
        };
    }
}