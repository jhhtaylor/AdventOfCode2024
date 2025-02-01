using System;
using System.IO;
using System.Text.RegularExpressions;

public class Day3Part2 : IDay
{
    public string Solve()
    {
        string filePath = "Inputs/Day3Input.txt";

        if (!File.Exists(filePath))
        {
            return "Error: input.txt not found!";
        }

        string memory = File.ReadAllText(filePath);
        int totalSum = 0;
        bool mulEnabled = true; // mul instructions are enabled by default

        // Regular expressions to match do(), don't(), and mul(X,Y) instructions
        Regex doRegex = new Regex(@"do\(\)");
        Regex dontRegex = new Regex(@"don't\(\)");
        Regex mulRegex = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)");

        // Find all matches in the memory string
        var matches = Regex.Matches(memory, @"(do\(\)|don't\(\)|mul\(\d{1,3},\d{1,3}\))");

        foreach (Match match in matches)
        {
            string instruction = match.Value;

            if (doRegex.IsMatch(instruction))
            {
                // Enable mul instructions
                mulEnabled = true;
            }
            else if (dontRegex.IsMatch(instruction))
            {
                // Disable mul instructions
                mulEnabled = false;
            }
            else if (mulRegex.IsMatch(instruction) && mulEnabled)
            {
                // Process enabled mul(X,Y) instructions
                var mulMatch = mulRegex.Match(instruction);
                if (mulMatch.Groups.Count == 3) // Ensure we have both X and Y
                {
                    int x = int.Parse(mulMatch.Groups[1].Value);
                    int y = int.Parse(mulMatch.Groups[2].Value);
                    totalSum += x * y;
                }
            }
        }

        return totalSum.ToString();
    }
}