using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day2Part2 : IDay
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

            if (IsSafeReportWithDampener(levels))
            {
                safeReports++;
            }
        }

        return safeReports.ToString();
    }

    private bool IsSafeReportWithDampener(List<int> levels)
    {
        if (levels.Count < 2)
            return true; // Single-level reports are safe by default

        // Check if the report is already safe without removing any level
        if (IsSafeReport(levels))
            return true;

        // Try removing each level one by one and check if the modified report is safe
        for (int i = 0; i < levels.Count; i++)
        {
            var modifiedLevels = new List<int>(levels);
            modifiedLevels.RemoveAt(i); // Remove the i-th level

            if (IsSafeReport(modifiedLevels))
                return true;
        }

        // If no single removal makes the report safe, it's unsafe
        return false;
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