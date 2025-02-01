using System;
using System.Collections.Generic;
using System.IO;

public class Day4Part2 : IDay
{
    public string Solve()
    {
        // Path to the input file
        string filePath = "Inputs/Day4Input.txt";
        if (!File.Exists(filePath))
            return "Error: input.txt not found!";

        // Read the grid from the input file
        List<string> lines = new List<string>();
        foreach (var line in File.ReadLines(filePath))
        {
            lines.Add(line);
        }

        int height = lines.Count;
        if (height == 0)
            return "0"; // If the grid is empty, return 0
        int width = lines[0].Length;

        // Convert the grid to a 2D array for easier access
        char[,] grid = new char[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, y] = lines[y][x];
            }
        }

        // Define the four diagonal directions to search for "MAS" sequences
        var diagonalDirections = new List<(int dx, int dy)>()
        {
            (1, 1),    // Down-Right
            (1, -1),   // Down-Left
            (-1, 1),   // Up-Right
            (-1, -1)   // Up-Left
        };

        int count = 0; // Counter for the number of X-MAS patterns

        // Iterate through each cell in the grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Check if the current cell contains 'A'
                if (grid[x, y] != 'A')
                    continue;

                int diagonalCount = 0; // Counter for the number of valid "MAS" sequences intersecting at 'A'

                // Check each diagonal direction for a "MAS" sequence
                foreach (var dir in diagonalDirections)
                {
                    int dx = dir.dx; // Row direction (e.g., 1 for down, -1 for up)
                    int dy = dir.dy; // Column direction (e.g., 1 for right, -1 for left)

                    // Starting point is one step in the opposite direction
                    int startX = x - dx;
                    int startY = y - dy;

                    // Check if the sequence "MAS" exists in this direction
                    if (SearchWord(grid, startX, startY, dx, dy, "MAS"))
                    {
                        diagonalCount++; // Increment the counter if a valid sequence is found
                    }
                }

                // If exactly two diagonal sequences intersect at 'A', it's a valid X-MAS pattern
                if (diagonalCount == 2)
                {
                    count++; // Increment the total count of X-MAS patterns
                }
            }
        }

        return count.ToString(); // Return the total count of X-MAS patterns
    }

    /// <summary>
    /// Searches for a word in the grid starting from a specific position and direction.
    /// </summary>
    /// <param name="grid">The 2D grid of characters.</param>
    /// <param name="startX">The starting X position.</param>
    /// <param name="startY">The starting Y position.</param>
    /// <param name="dx">The row direction.</param>
    /// <param name="dy">The column direction.</param>
    /// <param name="word">The word to search for.</param>
    /// <returns>True if the word is found, otherwise false.</returns>
    private bool SearchWord(char[,] grid, int startX, int startY, int dx, int dy, string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            int currentX = startX + dx * i; // Current X position
            int currentY = startY + dy * i; // Current Y position

            // Check if the current position is within the grid bounds
            if (currentX < 0 || currentX >= grid.GetLength(0) || currentY < 0 || currentY >= grid.GetLength(1))
                return false; // Out of bounds

            // Check if the character matches the corresponding character in the word
            if (grid[currentX, currentY] != word[i])
                return false; // Mismatch
        }

        return true; // The word is found
    }
}