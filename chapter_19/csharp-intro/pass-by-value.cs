using System.Globalization;

int n = 5;
Console.WriteLine(ValueDemo.Bump(n));
Console.WriteLine(n);
System.Console.WriteLine("-----------------------------------------------------------------------");
int o = 2;
int p = 3;
System.Console.WriteLine($"Before calling Swap: {o} - {p}");
RefOps.Swap(ref o, ref p);
System.Console.WriteLine($"After calling Swap: {o} - {p}");
System.Console.WriteLine("-----------------------------------------------------------------------");
Account account = new Account();
account.Balance = 100m;
System.Console.WriteLine(account.Balance);
AccountOps.Deposit(account, 50);
System.Console.WriteLine(account.Balance);
AccountOps.Reset(account);
System.Console.WriteLine(account.Balance);
System.Console.WriteLine("-----------------------------------------------------------------------");
System.Console.WriteLine(Parser.TryParsePrice("12.50", out decimal price) ? $"Valid - {price}" : $"Invalid - {price}");
System.Console.WriteLine(Parser.TryParsePrice("£12.50", out price) ? $"Valid - {price}" : $"Invalid - {price}");
System.Console.WriteLine(Parser.TryParsePrice("12,50", out price) ? $"Valid - {price}" : $"Invalid - {price}");
System.Console.WriteLine(Parser.TryParsePrice("hello", out price) ? $"Valid - {price}" : $"Invalid - {price}");
System.Console.WriteLine("-----------------------------------------------------------------------");
TemperatureRange range = new TemperatureRange(18, 24);
Console.WriteLine(RangeChecker.Contains(in range, 22));
Console.WriteLine(RangeChecker.Contains(in range, 16));
Console.WriteLine(RangeChecker.Contains(in range, 30));



public static class ValueDemo
{
    public static string Bump(int number)
    {
        number++;
        return $"{number}";
    }
}

public class RefOps
{
    public static void Swap(ref int a, ref int b)
    {
        int c;
        c = a;
        a = b;
        b = c;
    }
}

public class Account
{
    // A property: a value stored on each Account object that can be read and changed
    public decimal Balance { get; set; }
}

// A struct groups values like a class, but is copied when passed by value (like an int).
// readonly means its values cannot change after the constructor runs.
public readonly struct TemperatureRange
{
    public double Min { get; }
    public double Max { get; }

    public TemperatureRange(double min, double max)
    {
        Min = min;
        Max = max;
    }
}

public class RangeChecker
{
    public static bool Contains(in TemperatureRange range, double value)
    {
        if (value < range.Min || value > range.Max)
        {
            return false;
        }
        else return true;
    }
}

public class AccountOps
{
    public static void Deposit(Account account, decimal amount)             // Deposit modifies the original Account object because                                                                          
    {                                                                       // both references point to the same instance.
        account.Balance += amount;
    }

    public static void Reset(Account account)                               // Reset only changes the local copy of the reference.
    {                                                                       // The caller still points to the original Account object.
        account = new Account();
    }
}

public class Parser
{
    public static bool TryParsePrice(string text, out decimal price)
    {
        string cleaned = text.Trim();
        cleaned = cleaned.Replace("£", "");
        cleaned = cleaned.Replace(',', '.');
        return decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out price);
    }
}