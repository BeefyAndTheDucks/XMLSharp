using System.Text;

namespace Common;

public static class ArrayUtils
{
    extension<T>(T[] array)
    {
        public string GetTextForPrettyPrint(Func<T, string>? formatter = null)
        {
            StringBuilder sb = new StringBuilder();
            int lengthNumDigits = array.Length.ToString().Length;
        
            for (int i = 0; i < array.Length; i++)
            {
                string number = i.ToString().PadLeft(lengthNumDigits);
                string value = formatter?.Invoke(array[i]) ?? array[i]?.ToString() ?? "null";
                sb.Append('[');
                sb.Append(number);
                sb.Append("] = ");
                sb.Append(value);
                sb.Append('\n');
            }
            return sb.ToString();
        }

        public void PrettyPrint(Func<T, string>? formatter = null)
        {
            Console.WriteLine(array.GetTextForPrettyPrint(formatter));
        }
    }
}