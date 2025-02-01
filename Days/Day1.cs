using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day1 : IDay
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
        
        // Sort both lists in ascending order
        leftNumbers.Sort();
        rightNumbers.Sort();
        
        // Variable to store the total sum of absolute differences
        int totalSum = 0;
        
        // Iterate through the sorted lists and calculate the absolute difference for each pair
        for (int i = 0; i < leftNumbers.Count; i++)
        {
            int diff = Math.Abs(leftNumbers[i] - rightNumbers[i]); // Absolute difference
            totalSum += diff; // Add the difference to the total sum
        }
        
        // Return the total sum as a string
        return totalSum.ToString();
    }
}