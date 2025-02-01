using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day5Part2 : IDay
{
    public string Solve()
    {
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

        // Identify invalid updates (those that violate at least one rule)
        List<List<string>> invalidUpdates = new List<List<string>>();
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

            // If the update is invalid, add it to the list of invalid updates
            if (!isValid)
                invalidUpdates.Add(update);
        }

        // Sum the middle page numbers of the corrected updates
        int sum = 0;
        foreach (var invalidUpdate in invalidUpdates)
        {
            // Correct the order of the invalid update using topological sorting
            var correctedOrder = CorrectOrder(invalidUpdate, rules);

            // Find the middle page number
            int middleIndex = (correctedOrder.Count - 1) / 2; // Middle index for odd-length lists
            sum += int.Parse(correctedOrder[middleIndex]); // Add the middle page number to the sum
        }

        return sum.ToString();
    }

    /// <summary>
    /// Corrects the order of an update using topological sorting.
    /// </summary>
    /// <param name="update">The invalid update to correct.</param>
    /// <param name="rules">The list of rules (dependencies).</param>
    /// <returns>The corrected order of pages.</returns>
    private List<string> CorrectOrder(List<string> update, List<Tuple<string, string>> rules)
    {
        // Store the original indices of pages for tie-breaking during sorting
        var originalIndices = new Dictionary<string, int>();
        for (int i = 0; i < update.Count; i++)
            originalIndices[update[i]] = i;

        // Build the adjacency list and in-degree map for topological sorting
        var adjacency = new Dictionary<string, List<string>>();
        var inDegree = new Dictionary<string, int>();
        foreach (var page in update)
        {
            adjacency[page] = new List<string>(); // Initialize adjacency list for each page
            inDegree[page] = 0; // Initialize in-degree for each page
        }

        // Add edges based on the rules
        foreach (var rule in rules)
        {
            string x = rule.Item1; // X must come before Y
            string y = rule.Item2;

            // Only add the edge if both X and Y are in the update
            if (originalIndices.ContainsKey(x) && originalIndices.ContainsKey(y))
            {
                adjacency[x].Add(y); // Add Y to X's adjacency list
                inDegree[y]++; // Increment Y's in-degree
            }
        }

        // Initialize the queue with pages that have no dependencies (in-degree = 0)
        var queue = new List<string>();
        foreach (var page in update)
            if (inDegree[page] == 0)
                queue.Add(page);

        // Sort the queue by original indices to preserve the original order when possible
        queue.Sort((a, b) => originalIndices[a].CompareTo(originalIndices[b]));

        // Perform topological sorting
        List<string> result = new List<string>();
        while (queue.Count > 0)
        {
            string node = queue[0]; // Take the first page from the queue
            queue.RemoveAt(0);
            result.Add(node); // Add it to the result

            // Process all neighbors (pages that depend on the current page)
            foreach (var neighbor in adjacency[node])
            {
                inDegree[neighbor]--; // Decrement the neighbor's in-degree
                if (inDegree[neighbor] == 0) // If the neighbor has no more dependencies, add it to the queue
                {
                    queue.Add(neighbor);
                    queue.Sort((a, b) => originalIndices[a].CompareTo(originalIndices[b])); // Sort by original indices
                }
            }
        }

        return result;
    }
}