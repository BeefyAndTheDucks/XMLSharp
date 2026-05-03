namespace XMLSharpInterpreter;

using System.Text.Json;
using Common;

public static class InstructionHelper
{
    public static object RequireData(IRInstruction instruction)
    {
        var data = instruction.Data ?? throw new InvalidOperationException("Instruction has no data.");
        return data is JsonElement element
            ? element.ValueKind switch
            {
                JsonValueKind.Number => element.GetInt32(),
                JsonValueKind.String => element.GetString()!,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => throw new InvalidOperationException($"Unsupported JSON value kind: {element.ValueKind}")
            }
            : data;
    }
}