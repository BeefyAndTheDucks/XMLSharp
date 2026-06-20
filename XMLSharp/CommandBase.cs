using System.CommandLine;

namespace XMLSharp;

public abstract class CommandBase
{
    protected abstract void Invoke(ParseResult parseResult);

    protected abstract Command GetCommand();

    public Command CreateCommand()
    {
        Command command = GetCommand();
        command.SetAction(Invoke);

        return command;
    }
}