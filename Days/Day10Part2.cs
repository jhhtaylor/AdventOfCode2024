// Days/Day10HoofItPart2.cs
using System;
using System.IO;

public class Day10Part2 : IDay
{
    // We'll use a memoization table to cache the number of paths from each cell.
    private long[,] memo;

    // Dimensions of the grid.
    private int rows, cols;

    // The grid of heights.
    private int[,] grid;

    public string Solve()
    {
        // Read the topographic map input.
        // The input file should contain one row per line where each character is a digit (0 through 9).
        string filePath = "Inputs/Day10Input.txt";
        if (!File.Exists(filePath))
        {
            return "Error: Input file not found.";
        }
        string[] lines = File.ReadAllLines(filePath);
        rows = lines.Length;
        if (rows == 0)
            return "Error: Input file is empty.";
        cols = lines[0].Length;
        
        grid = new int[rows, cols];
        for (int r = 0; r < rows; r++)
        {
            if (lines[r].Length != cols)
                return "Error: Not all lines have the same length.";
            for (int c = 0; c < cols; c++)
            {
                // Convert each character to its integer height.
                grid[r, c] = lines[r][c] - '0';
            }
        }
        
        // Set up the memoization array.
        memo = new long[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                memo[r, c] = -1;  // Use -1 to mark an uncomputed cell.
        
        long totalRating = 0;
        
        // A trailhead is any cell with height 0.
        // Process every cell in reading order and, if its height is 0, add its computed paths.
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] == 0)
                {
                    long trailsFromHere = CountTrails(r, c);
                    // For debugging you might print:
                    // Console.WriteLine($"Trailhead at ({r},{c}) has rating {trailsFromHere}");
                    totalRating += trailsFromHere;
                }
            }
        }
        
        return totalRating.ToString();
    }

    /// <summary>
    /// Returns the number of distinct hiking trails starting from cell (r, c) and
    /// eventually reaching a cell of height 9, following the rule that each step
    /// must go to an adjacent cell (up/down/left/right) with height exactly one more.
    /// </summary>
    private long CountTrails(int r, int c)
    {
        // If out-of-bounds (should not happen if called correctly) return 0.
        if (r < 0 || r >= rows || c < 0 || c >= cols)
            return 0;
        
        // If this cell has height 9, it is an endpoint.
        if (grid[r, c] == 9)
            return 1;
        
        // If we have computed this cell already, return the stored value.
        if (memo[r, c] != -1)
            return memo[r, c];
        
        long total = 0;
        int currentHeight = grid[r, c];
        int nextHeight = currentHeight + 1;
        
        // Define the 4 directions: up, down, left, right.
        int[] dr = { -1, 1, 0, 0 };
        int[] dc = { 0, 0, -1, 1 };
        
        for (int i = 0; i < 4; i++)
        {
            int nr = r + dr[i];
            int nc = c + dc[i];
            // Check bounds.
            if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
            {
                // Only move if the neighbor’s height is exactly one greater.
                if (grid[nr, nc] == nextHeight)
                {
                    total += CountTrails(nr, nc);
                }
            }
        }
        
        memo[r, c] = total;
        return total;
    }
}
