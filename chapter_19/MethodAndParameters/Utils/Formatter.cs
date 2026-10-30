namespace Utils;
public static class Formatter
{
    public static string FormatNumber(int n) => $"Number: {n}";
    public static string FormatNumber(double d) => $"Double: {d:F2}";

    public static string FormatMessage(string text, int repeat = 1)
    {
        string message = "";
        for(int i = 0; i < repeat; i++)
        {
            message += (text + " ");
        }
        return message.TrimEnd();
    }
}