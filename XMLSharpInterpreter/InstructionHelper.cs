namespace XMLSharpInterpreter;

using System.Text.Json;
using Common;

public static class InstructionHelper
{
    public static dynamic GetConstantValue(IRConstant constant)
    {
        dynamic data = constant.Value ?? throw new InvalidOperationException("Instruction has no data.");

        if (data is not JsonElement element)
            return data;

        switch (element.ValueKind)
        {
            case JsonValueKind.Number:
                try
                {
                    return element.GetInt32();
                }
                catch (FormatException)
                {
                    return element.GetSingle();
                }
            case JsonValueKind.String:
                return element.GetString()!;
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            default:
                throw new InvalidOperationException($"Unsupported JSON value kind: {element.ValueKind}");
        }
    }
    
    public static int JumpTo(int destinationInstructionIndex, int currentInstructionIndex)
    {
        return destinationInstructionIndex - currentInstructionIndex;
    }
}