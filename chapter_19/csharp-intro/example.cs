{
    int year = 2025;
    Console.WriteLine($"The year is {year}");
    for (int i = 0; i < 3; i++)
    {
        System.Console.WriteLine($"Count: {i}");
    }
}

System.Console.WriteLine("\nDistance: ");
double distance = 1.5e8;
System.Console.WriteLine(distance);

System.Console.WriteLine("\nOperators:");
int a = 10;
int b = 3;
string message = "Hey there!";
System.Console.WriteLine(a + message);

System.Console.WriteLine("\nTernary Operator:");
int temperature = 24;
string feeling = temperature > 20 ? "warm" : "cold";
System.Console.WriteLine(feeling);

System.Console.WriteLine("\nMath helpers:");
double pi = 3.14159;
System.Console.WriteLine(Math.Round(pi, 2));
System.Console.WriteLine(Math.Round(2.5, MidpointRounding.AwayFromZero));
System.Console.WriteLine(Math.Abs(-42));

decimal price = 19.456m;
System.Console.WriteLine(Math.Round(price, 2));