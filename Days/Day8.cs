using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day8 : IDay
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

        // Parse the grid to find antennas
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

        // Calculate antinodes
        var antinodes = new HashSet<(int x, int y)>();
        for (int i = 0; i < antennas.Count; i++)
        {
            for (int j = i + 1; j < antennas.Count; j++)
            {
                if (antennas[i].frequency == antennas[j].frequency)
                {
                    var (x1, y1, _) = antennas[i];
                    var (x2, y2, _) = antennas[j];

                    // Calculate the antinodes
                    var dx = x2 - x1;
                    var dy = y2 - y1;

                    // Antinode 1: (x1 - dx, y1 - dy)
                    var antinode1 = (x1 - dx, y1 - dy);
                    if (IsWithinBounds(antinode1, grid))
                    {
                        antinodes.Add(antinode1);
                    }

                    // Antinode 2: (x2 + dx, y2 + dy)
                    var antinode2 = (x2 + dx, y2 + dy);
                    if (IsWithinBounds(antinode2, grid))
                    {
                        antinodes.Add(antinode2);
                    }
                }
            }
        }

        // Return the count of unique antinodes
        return antinodes.Count.ToString();
    }

    /// <summary>
    /// Checks if a point is within the bounds of the grid.
    /// </summary>
    private bool IsWithinBounds((int x, int y) point, string[] grid)
    {
        return point.x >= 0 && point.x < grid[0].Length && point.y >= 0 && point.y < grid.Length;
    }
}