using System.IO.Pipelines;

int age = 42;
long worldPopulation = 8000000000;
float temperature = 37.6F;
double price = 24.50D;
System.Console.WriteLine($"Age: {age} - World population: {worldPopulation} - Temperature: {temperature} - Price: {price}");
System.Console.WriteLine("------------------------------------------------------------------");
int a = 10;
int b = 3;
System.Console.WriteLine($"Addition: {a + b}");
System.Console.WriteLine($"Substraction: {a - b}");
System.Console.WriteLine($"Multiplication: {a * b}");
System.Console.WriteLine($"Division: {a / b}");
System.Console.WriteLine($"Modulo: {a % b}");
System.Console.WriteLine(a < b);    // false
System.Console.WriteLine(a > b);    // true
System.Console.WriteLine(a == b);   // false
System.Console.WriteLine(a != b);   // true
System.Console.WriteLine(a > b && b == 3);  // true
System.Console.WriteLine(a < b || b == 3);  // true
System.Console.WriteLine(!(a < b));   // true
System.Console.WriteLine("------------------------------------------------------------------");
double distanceSun = 1496e5;
double electronMass = 9.1093837139e-31;
System.Console.WriteLine($"Distance to the sun: {distanceSun} - Electron mass: {electronMass}");    // Multiplication comes before addition, but parentheses always come first.
System.Console.WriteLine("------------------------------------------------------------------");
int result1 = 2 + 3 * 4;
int result2 = (2 + 3) * 4;
System.Console.WriteLine($"result1 = {result1} - result2 = {result2}");
System.Console.WriteLine("------------------------------------------------------------------");
int counter = 1;
counter += 1;
System.Console.WriteLine($"Counter after += 1: {counter}");
counter -= 1;
System.Console.WriteLine($"Counter after -= 1: {counter}");
counter *= 2;
System.Console.WriteLine($"Counter after *= 2: {counter}");
counter /= 2;
System.Console.WriteLine($"Counter after /= 2: {counter}");
counter++;
System.Console.WriteLine($"Counter after ++: {counter}"); 
System.Console.WriteLine("------------------------------------------------------------------");
double score = 7.85D;
int truncated = (int)score;
double rounded = Math.Round(score, 1);
float scoreF = (float)score;
System.Console.WriteLine($"score: {score} - truncated: {truncated} - rounded: {rounded} - scoreF: {scoreF}");
System.Console.WriteLine("------------------------------------------------------------------");
try
{
    checked
    {
        int max = int.MaxValue;
        Console.WriteLine($"Max int: {max}");
        // The next line will throw OverflowException if executed in checked context
        int boom = max + 1;
        Console.WriteLine($"This will not print (boom={boom})");
    }
}
catch (OverflowException)
{
    Console.WriteLine("Overflow caught in checked context");
}
unchecked
{
    int max = int.MaxValue;
    int wrapped = max + 1; // wraps to negative in unchecked context
    Console.WriteLine($"Unchecked wrap-around result: {wrapped}");
}
System.Console.WriteLine("------------------------------------------------------------------");
float f = 0.1F + 0.2F;
double d = 0.1D + 0.2D;
System.Console.WriteLine($"f: {f} - d: {d} - comparison: {f == d}");
System.Console.WriteLine("------------------------------------------------------------------");
double weightKg = 96;
double heightM = 1.89;
double heightSquared = heightM * heightM;
double result = weightKg / heightSquared;
string message = result < 18.5 || result > 24.9 ? "Not normal" : "Normal";
System.Console.WriteLine(message);