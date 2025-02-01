using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day2 : IDay
{
    public string Solve()
    {
        string filePath = "Inputs/Day2Input.txt";

        if (!File.Exists(filePath))
        {
            return "Error: input.txt not found!";
        }

        int safeReports = 0;

        foreach (string line in File.ReadLines(filePath))
        {
            var levels = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(int.Parse)
                             .ToList();

            //Console.WriteLine($"line: {line}");

            if (IsSafeReport(levels))
            {
                //Console.WriteLine($"Safe report: {string.Join(" ", levels)}");
                //Console.WriteLine("===================================");
                safeReports++;
            }
        }

        return safeReports.ToString();
    }

    private bool IsSafeReport(List<int> levels)
    {
        if (levels.Count < 2)
            return true; // Single-level reports are safe by default

        bool isIncreasing = true;
        bool isDecreasing = true;

        for (int i = 1; i < levels.Count; i++)
        {
            int diff = levels[i] - levels[i - 1];

            // Check if the difference is within the allowed range
            if (Math.Abs(diff) < 1 || Math.Abs(diff) > 3)
                return false;

            // Update whether the sequence is increasing or decreasing
            if (diff > 0)
                isDecreasing = false;
            if (diff < 0)
                isIncreasing = false;
        }

        // The sequence must be either strictly increasing or strictly decreasing
        return isIncreasing || isDecreasing;
    }
}