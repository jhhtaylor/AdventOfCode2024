using System;
using System.Collections.Generic;
using System.IO;

public class Day4 : IDay
{
    public string Solve()
    {
        // Path to the input file
        string filePath = "Inputs/Day4Input.txt";
        if (!File.Exists(filePath))
            return "Error: input.txt not found!";

        // Read the grid from the input file
        List<string> grid = new List<string>();
        foreach (var line in File.ReadLines(filePath))
        {
            grid.Add(line);
        }

        int rows = grid.Count;
        if (rows == 0)
            return "0"; // If the grid is empty, return 0
        int cols = grid[0].Length;

        // Define all possible directions (8 directions: horizontal, vertical, diagonal)
        List<Tuple<int, int>> directions = new List<Tuple<int, int>>()
        {
            Tuple.Create(0, 1),    // Right
            Tuple.Create(0, -1),   // Left
            Tuple.Create(1, 0),    // Down
            Tuple.Create(-1, 0),   // Up
            Tuple.Create(1, 1),    // Down-Right
            Tuple.Create(1, -1),   // Down-Left
            Tuple.Create(-1, 1),   // Up-Right
            Tuple.Create(-1, -1)   // Up-Left
        };

        int count = 0; // Counter for the number of "XMAS" occurrences

        // Iterate through each cell in the grid
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Check each direction for the word "XMAS"
                foreach (var dir in directions)
                {
                    int dr = dir.Item1; // Row direction (e.g., 1 for down, -1 for up)
                    int dc = dir.Item2; // Column direction (e.g., 1 for right, -1 for left)

                    // Calculate the end position of the sequence
                    int endRow = i + 3 * dr;
                    int endCol = j + 3 * dc;

                    // Check if the end position is within the grid bounds
                    if (endRow < 0 || endRow >= rows || endCol < 0 || endCol >= cols)
                        continue; // Skip if the sequence goes out of bounds

                    // Collect the four characters in the current direction
                    char[] chars = new char[4];
                    chars[0] = grid[i][j];         // Starting character
                    chars[1] = grid[i + dr][j + dc];       // Second character
                    chars[2] = grid[i + 2 * dr][j + 2 * dc]; // Third character
                    chars[3] = grid[i + 3 * dr][j + 3 * dc]; // Fourth character

                    // Convert the characters to a string
                    string sequence = new string(chars);

                    // Check if the sequence matches "XMAS"
                    if (sequence == "XMAS")
                    {
                        count++; // Increment the count if a match is found
                    }
                }
            }
        }

        return count.ToString(); // Return the total count of "XMAS" occurrences
    }
}