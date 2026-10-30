using Utils;

Console.WriteLine($"Add: {Calculator.Add(4,5)}");
System.Console.WriteLine($"Multiply: {Calculator.Multiply(4,5)}");
System.Console.WriteLine($"Divide: {Calculator.Divide(4,5)}");
System.Console.WriteLine($"Average: {Statistics.Average(4,5)}");
System.Console.WriteLine($"Average: {Statistics.Average(4,5,6)}");
System.Console.WriteLine(Formatter.FormatNumber(29));
System.Console.WriteLine(Formatter.FormatNumber(29.4449));
System.Console.WriteLine(Formatter.FormatMessage("Hi"));
System.Console.WriteLine(Formatter.FormatMessage("Hi", repeat: 3));