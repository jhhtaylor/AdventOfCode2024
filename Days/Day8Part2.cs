using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day8Part2 : IDay
{
    public string Solve()
    {
        // Path to the input file
        string filePath = "Inputs/Day8Input.txt";

        // Check if the input file exists
        if (!File.Exists(filePath))
        {
            return "Error: input.txt not found!";
        }

        // Read the input grid
        var grid = File.ReadAllLines(filePath);

        // Parse the grid to find antennas.
        // Antennas are represented by any character other than '.'
        var antennas = new List<(int x, int y, char frequency)>();
        for (int y = 0; y < grid.Length; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                if (grid[y][x] != '.')
                {
                    antennas.Add((x, y, grid[y][x]));
                }
            }
        }

        // Console.WriteLine("Antennas:");
        // foreach (var antenna in antennas)
        // {
        //     Console.WriteLine($"({antenna.x}, {antenna.y}, {antenna.frequency})");
        // }

        // Group antennas by frequency
        var frequencyGroups = antennas.GroupBy(a => a.frequency);

        // Use a HashSet to keep track of unique antinode positions.
        var antinodes = new HashSet<(int x, int y)>();

        // For every pair of antennas of the same frequency, calculate the extended line.
        foreach (var group in frequencyGroups)
        {
            var antennasOfFrequency = group.ToList();
            if (antennasOfFrequency.Count < 2)
                continue; // no pair => no antinode for this frequency

            for (int i = 0; i < antennasOfFrequency.Count; i++)
            {
                for (int j = i + 1; j < antennasOfFrequency.Count; j++)
                {
                    var (x1, y1, _) = antennasOfFrequency[i];
                    var (x2, y2, _) = antennasOfFrequency[j];

                    // Get all positions on the infinite line (within grid bounds)
                    // defined by these two antennas.
                    var linePositions = GetExtendedLinePositions(x1, y1, x2, y2, grid);
                    //Console.WriteLine($"Line between ({x1}, {y1}) and ({x2}, {y2}):");
                    foreach (var pos in linePositions)
                    {
                        //Console.WriteLine($"({pos.x}, {pos.y})");
                        antinodes.Add(pos);
                    }
                }
            }
        }

        //Console.WriteLine($"Total antinodes: {antinodes.Count}");
        return antinodes.Count.ToString();
    }

    /// <summary>
    /// Returns all grid positions (within the bounds of the grid) that lie on the infinite line
    /// defined by (x1, y1) and (x2, y2).
    /// </summary>
    private List<(int x, int y)> GetExtendedLinePositions(int x1, int y1, int x2, int y2, string[] grid)
    {
        var positions = new List<(int x, int y)>();

        int dx = x2 - x1;
        int dy = y2 - y1;
        int g = GCD(Math.Abs(dx), Math.Abs(dy));
        
        // If the antennas are at the same location, just return that point.
        if (g == 0)
        {
            if (IsWithinBounds((x1, y1), grid))
                positions.Add((x1, y1));
            return positions;
        }

        // Calculate the smallest (primitive) step in each direction.
        int stepX = dx / g;
        int stepY = dy / g;

        // Find the starting point by stepping backwards until we're out of bounds.
        int startX = x1;
        int startY = y1;
        while (true)
        {
            int nextX = startX - stepX;
            int nextY = startY - stepY;
            if (!IsWithinBounds((nextX, nextY), grid))
                break;
            startX = nextX;
            startY = nextY;
        }

        // Now, step forward from the starting point until we exit the grid bounds.
        int curX = startX;
        int curY = startY;
        while (IsWithinBounds((curX, curY), grid))
        {
            positions.Add((curX, curY));
            curX += stepX;
            curY += stepY;
        }

        return positions;
    }

    /// <summary>
    /// Calculates the greatest common divisor of two integers using Euclid's algorithm.
    /// </summary>
    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    /// <summary>
    /// Checks if a point is within the bounds of the grid.
    /// </summary>
    private bool IsWithinBounds((int x, int y) point, string[] grid)
    {
        return point.x >= 0 && point.x < grid[0].Length && point.y >= 0 && point.y < grid.Length;
    }
}
