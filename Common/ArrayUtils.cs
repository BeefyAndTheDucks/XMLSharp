namespace Common;

public static class ArrayUtils
{
    public static void PrettyPrint<T>(this T[] array, Func<T, string>? formatter = null)
    {
        int lengthNumDigits = array.Length.ToString().Length;
        
        for (int i = 0; i < array.Length; i++)
        {
            string number = i.ToString().PadLeft(lengthNumDigits);
            string value = formatter?.Invoke(array[i]) ?? array[i]?.ToString() ?? "null";
            Console.WriteLine($"[{number}] = {value}");
        }
    }
}