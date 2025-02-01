using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day5 : IDay
{
    public string Solve()
    {
        // Path to the input file
        string filePath = "Inputs/Day5Input.txt";
        if (!File.Exists(filePath))
            return "Error: input.txt not found!";

        // Read all lines from the input file
        var lines = File.ReadAllLines(filePath).ToList();

        // Find the separator between rules and updates (an empty line)
        int separatorIndex = lines.FindIndex(string.IsNullOrWhiteSpace);
        if (separatorIndex == -1)
            return "Error: input format incorrect.";

        // Split the input into rules and updates
        var ruleLines = lines.Take(separatorIndex).ToList(); // Rules are before the separator
        var updateLines = lines.Skip(separatorIndex + 1).Where(l => !string.IsNullOrWhiteSpace(l)).ToList(); // Updates are after the separator

        // Parse the rules into a list of tuples (X, Y), where X must come before Y
        List<Tuple<string, string>> rules = new List<Tuple<string, string>>();
        foreach (var line in ruleLines)
        {
            var parts = line.Split('|');
            if (parts.Length != 2)
                continue; // Skip invalid lines
            rules.Add(Tuple.Create(parts[0].Trim(), parts[1].Trim()));
        }

        // Parse the updates into a list of lists (each update is a list of page numbers)
        List<List<string>> updates = new List<List<string>>();
        foreach (var line in updateLines)
        {
            var pages = line.Split(',').Select(p => p.Trim()).ToList();
            updates.Add(pages);
        }

        // Sum the middle page numbers of the correctly ordered updates
        int sum = 0;

        // Check each update to see if it is correctly ordered
        foreach (var update in updates)
        {
            var pagesSet = new HashSet<string>(update); // Use a set for quick lookup
            bool isValid = true;

            // Check each rule to see if it is violated in this update
            foreach (var rule in rules)
            {
                string x = rule.Item1; // Page X must come before page Y
                string y = rule.Item2;

                // If both X and Y are in the update, check their order
                if (pagesSet.Contains(x) && pagesSet.Contains(y))
                {
                    int indexX = update.IndexOf(x); // Position of X in the update
                    int indexY = update.IndexOf(y); // Position of Y in the update

                    // If X comes after Y, the update is invalid
                    if (indexX >= indexY)
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            // If the update is valid, add its middle page number to the sum
            if (isValid)
            {
                int middleIndex = (update.Count - 1) / 2; // Middle index for odd-length lists
                if (int.TryParse(update[middleIndex], out int middlePage))
                {
                    sum += middlePage; // Add the middle page number to the sum
                }
            }
        }

        return sum.ToString();
    }
}