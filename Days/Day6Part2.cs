using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day6Part2 : IDay
{
    public string Solve()
    {
        string filePath = "Inputs/Day6Input.txt";
        
        if (!File.Exists(filePath))
            return "Error: input.txt not found!";

        // Read the map from the input file
        List<string> map = File.ReadAllLines(filePath).ToList();

        // Find the guard's starting position and initial direction
        int guardX = -1, guardY = -1;
        char guardDirection = ' ';
        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                char cell = map[y][x];
                if (cell == '^' || cell == 'v' || cell == '<' || cell == '>')
                {
                    guardX = x;
                    guardY = y;
                    guardDirection = cell;
                    break;
                }
            }
            if (guardX != -1)
                break;
        }

        if (guardX == -1)
            return "Error: Guard's starting position not found!";

        // Define direction vectors: up, right, down, left
        var directions = new List<(int dx, int dy, char symbol)>()
        {
            (0, -1, '^'), // Up
            (1, 0, '>'),  // Right
            (0, 1, 'v'),  // Down
            (-1, 0, '<')  // Left
        };

        // Find the index of the guard's initial direction
        int directionIndex = directions.FindIndex(d => d.symbol == guardDirection);

        // List to store valid obstruction positions
        List<(int x, int y)> validObstructionPositions = new List<(int x, int y)>();

        // Iterate through each cell in the grid
        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                // Skip the guard's starting position and existing obstacles
                if ((x == guardX && y == guardY) || map[y][x] == '#')
                    continue;

                // Temporarily place an obstruction
                var tempMap = map.Select(row => row.ToCharArray()).ToList();
                tempMap[y][x] = '#';

                // Simulate the guard's movement and check for a loop
                if (IsLoopDetected(tempMap, guardX, guardY, directionIndex))
                {
                    validObstructionPositions.Add((x, y));
                }
            }
        }

        // Return the number of valid obstruction positions
        return validObstructionPositions.Count.ToString();
    }

    /// <summary>
    /// Simulates the guard's movement and checks if a loop is detected.
    /// </summary>
    private bool IsLoopDetected(List<char[]> map, int startX, int startY, int startDirectionIndex)
    {
        // Set to track visited states (position + direction)
        HashSet<(int x, int y, int directionIndex)> visitedStates = new HashSet<(int x, int y, int directionIndex)>();

        int guardX = startX;
        int guardY = startY;
        int directionIndex = startDirectionIndex;

        // Define direction vectors: up, right, down, left
        var directions = new List<(int dx, int dy, char symbol)>()
        {
            (0, -1, '^'), // Up
            (1, 0, '>'),  // Right
            (0, 1, 'v'),  // Down
            (-1, 0, '<')  // Left
        };

        // Simulate the guard's movement
        while (true)
        {
            // Get the current direction
            var (dx, dy, _) = directions[directionIndex];

            // Calculate the next position
            int nextX = guardX + dx;
            int nextY = guardY + dy;

            // Check if the next position is outside the map
            if (nextX < 0 || nextX >= map[0].Length || nextY < 0 || nextY >= map.Count)
                return false; // Guard has left the map

            // Check if there is an obstacle at the next position
            if (map[nextY][nextX] == '#')
            {
                // Turn right 90 degrees
                directionIndex = (directionIndex + 1) % 4;
            }
            else
            {
                // Move forward
                guardX = nextX;
                guardY = nextY;
            }

            // Check if the current state has been visited before
            var currentState = (guardX, guardY, directionIndex);
            if (visitedStates.Contains(currentState))
                return true; // Loop detected

            // Add the current state to the set of visited states
            visitedStates.Add(currentState);
        }
    }
}