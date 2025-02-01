using System;
using System.IO;
using System.Text.RegularExpressions;

public class Day3 : IDay
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

        // Regular expression to match valid mul(X,Y) instructions
        Regex regex = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)");
        MatchCollection matches = regex.Matches(memory);

        foreach (Match match in matches)
        {
            if (match.Groups.Count == 3) // Ensure we have both X and Y
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                totalSum += x * y;
            }
        }

        return totalSum.ToString();
    }
}