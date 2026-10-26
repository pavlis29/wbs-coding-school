while(true)
{
    System.Console.Write("What is your age? ");
    string? input = Console.ReadLine();
    if(int.TryParse(input, out int age))
    {
        if (age < 0)
        {
            System.Console.WriteLine("Age cannot be negative!\n");
            continue;
        }
        // if (age < 13)
        // {
        //     System.Console.WriteLine($"You are a child.");
        // }
        // else if (age >= 13 && age < 20)
        // {
        //     System.Console.WriteLine("You are a teenager.");
        // }
        // else
        // {
        //     System.Console.WriteLine("You are an adult.");
        // }
        string message = age < 13 ? "You are a child." : (age < 20 ? "You are a teenager." : "You are an adult.");
        System.Console.WriteLine(message);
        break;
    }
    else
    {
        System.Console.WriteLine("Enter a valid number.");
    }
}
System.Console.WriteLine("---------------------------------------------------------------------------------------------------------------");
System.Console.Write("Enter your grade (A,B,C,D,F)");
string? input2 = System.Console.ReadLine();
input2 ??= "Unknown";
switch(input2.Trim().ToUpperInvariant())
{
    case "A": System.Console.WriteLine("Excellent");
    break;
    case "B": System.Console.WriteLine("Good");
    break;
    case "C": System.Console.WriteLine("Average");
    break;
    case "D": System.Console.WriteLine("Bellow average");
    break;
    case "F": System.Console.WriteLine("Fail");
    break;
    default: System.Console.WriteLine("Unknown grade");
    break;
}
System.Console.WriteLine("---------------------------------------------------------------------------------------------------------------");
int i = 1;
int sum = 0;
while(i <= 100)
{
    sum += i;
    i++;
}
System.Console.WriteLine(sum);
System.Console.WriteLine("---------------------------------------------------------------------------------------------------------------");
int number = 29;
int n;
do
{
    System.Console.Write("Picks the number: ");
    string? input3 = Console.ReadLine();
    if(int.TryParse(input3, out n))
    {
        string message = n < number ? "Too low!" : (n == number ? "Correct" : "Too high");
        System.Console.WriteLine(message);
    }
}
while(n != number);
System.Console.WriteLine("---------------------------------------------------------------------------------------------------------------");
for(i = 1; i <= 10; i++)
{
    System.Console.WriteLine(i * 7);
}
System.Console.WriteLine("---------------------------------------------------------------------------------------------------------------");
for(i = 1; i <= 20; i++)
{
    if(i % 2 == 0)
    {
        continue;
    }
    if(i == 15)
    {
        break;
    }
    System.Console.WriteLine(i);
}