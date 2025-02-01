// Program.cs
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Select day to run (e.g., 1 for Day1): ");
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int dayNumber))
        {
            IDay? daySolution = DayFactory.GetDaySolution(dayNumber);
            if (daySolution != null)
            {
                Console.WriteLine(daySolution.Solve());
            }
            else
            {
                Console.WriteLine("Invalid day selected or not implemented yet.");
            }
        }
    }
}