using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day1Part2 : IDay
{
    public string Solve()
    {
        // Path to the input file
        string filePath = "Inputs/Day1Input.txt";
        
        // Check if the input file exists
        if (!File.Exists(filePath))
        {
            return "Error: input.txt not found!";
        }
        
        // Lists to store the left and right numbers
        List<int> leftNumbers = new List<int>();
        List<int> rightNumbers = new List<int>();
        
        // Read the input file line by line
        foreach (string line in File.ReadLines(filePath))
        {
            // Split each line into parts based on spaces or tabs
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Ensure the line has exactly two numbers
            if (parts.Length == 2 && int.TryParse(parts[0], out int left) && int.TryParse(parts[1], out int right))
            {
                // Add the left number to the leftNumbers list
                leftNumbers.Add(left);
                // Add the right number to the rightNumbers list
                rightNumbers.Add(right);
            }
        }
        
        // Dictionary to store the frequency of each number in the right list
        Dictionary<int, int> rightFrequency = new Dictionary<int, int>();
        
        // Count the frequency of each number in the right list
        foreach (int num in rightNumbers)
        {
            if (rightFrequency.ContainsKey(num))
                rightFrequency[num]++; // Increment the count if the number already exists
            else
                rightFrequency[num] = 1; // Initialize the count if the number is new
        }
        
        // Variable to store the similarity score
        int similarityScore = 0;
        
        // Calculate the similarity score
        foreach (int num in leftNumbers)
        {
            // Check if the number exists in the rightFrequency dictionary
            if (rightFrequency.TryGetValue(num, out int count))
            {
                // Add the product of the number and its frequency to the similarity score
                similarityScore += num * count;
            }
        }
        
        // Return the similarity score as a string
        return similarityScore.ToString();
    }
}