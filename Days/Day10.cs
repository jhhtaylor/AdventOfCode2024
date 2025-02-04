// Days/Day10HoofIt.cs
using System;
using System.Collections.Generic;
using System.IO;

public class Day10 : IDay
{
    public string Solve()
    {
        // Read the topographic map input.
        // The input is a multi-line file; each line is a row of digits (0 through 9).
        string filePath = "Inputs/Day10Input.txt";
        if (!File.Exists(filePath))
        {
            return "Error: Input file not found.";
        }
        string[] lines = File.ReadAllLines(filePath);
        int rows = lines.Length;
        if (rows == 0)
            return "Error: Input file is empty.";
        int cols = lines[0].Length;
        
        // Create a grid of heights.
        int[,] grid = new int[rows, cols];
        for (int r = 0; r < rows; r++)
        {
            if (lines[r].Length != cols)
            {
                return "Error: Not all lines have the same length.";
            }
            for (int c = 0; c < cols; c++)
            {
                // Convert the digit character to an integer.
                grid[r, c] = lines[r][c] - '0';
            }
        }
        
        int totalScore = 0;
        // For each cell that is a trailhead (height 0), perform a BFS to find reachable 9's.
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] == 0)
                {
                    // For each trailhead, find all distinct positions (cells) of height 9
                    // that can be reached following a path that increases exactly by 1 at each step.
                    HashSet<(int r, int c)> reachableNines = FindReachableNines(grid, r, c, rows, cols);
                    int trailheadScore = reachableNines.Count;
                    totalScore += trailheadScore;
                }
            }
        }
        
        return totalScore.ToString();
    }
    
    /// <summary>
    /// Starting from (startR, startC) – which must be height 0 – this method performs a BFS
    /// following only moves where the next cell’s height is exactly (current + 1).
    /// It returns a set of all positions (row, col) that have height 9 reached along such a trail.
    /// </summary>
    private HashSet<(int, int)> FindReachableNines(int[,] grid, int startR, int startC, int rows, int cols)
    {
        var reachableNines = new HashSet<(int, int)>();
        bool[,] visited = new bool[rows, cols];
        var queue = new Queue<(int r, int c)>();
        
        // Start at the trailhead.
        queue.Enqueue((startR, startC));
        visited[startR, startC] = true;
        
        // Only 4-directional moves: up, down, left, right.
        int[] dr = new int[] { -1, 1, 0, 0 };
        int[] dc = new int[] { 0, 0, -1, 1 };
        
        while (queue.Count > 0)
        {
            (int r, int c) = queue.Dequeue();
            int currentHeight = grid[r, c];
            
            // If we've reached a cell of height 9, record it.
            if (currentHeight == 9)
            {
                reachableNines.Add((r, c));
                // Do not expand further from a height-9 cell (there is no height 10).
                continue;
            }
            
            int nextHeight = currentHeight + 1;
            // Check each of the four adjacent cells.
            for (int i = 0; i < 4; i++)
            {
                int nr = r + dr[i];
                int nc = c + dc[i];
                if (nr < 0 || nr >= rows || nc < 0 || nc >= cols)
                    continue;
                // Only follow the path if the neighbor’s height is exactly current + 1.
                if (!visited[nr, nc] && grid[nr, nc] == nextHeight)
                {
                    visited[nr, nc] = true;
                    queue.Enqueue((nr, nc));
                }
            }
        }
        return reachableNines;
    }
}
