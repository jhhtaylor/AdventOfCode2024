// Days/Day6Visualization.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

public class Day6Visualization : IDay
{
    public string Solve()
    {
        string filePath = "Inputs/Day6Input_Example.txt";
        
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

        // Set to store distinct positions visited by the guard
        HashSet<(int x, int y)> visitedPositions = new HashSet<(int x, int y)>();
        visitedPositions.Add((guardX, guardY)); // Include the starting position

        // Directory to save frames
        string framesDir = "Day6Frames";
        Directory.CreateDirectory(framesDir);

        // Frame counter
        int frameNumber = 0;

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
                break; // Guard has left the map

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
                visitedPositions.Add((guardX, guardY)); // Add the new position to the set
            }

            // Generate and save the current frame
            SaveFrame(map, guardX, guardY, directions[directionIndex].symbol, visitedPositions, Path.Combine(framesDir, $"frame_{frameNumber:0000}.png"));
            frameNumber++;
        }

        // Compile frames into a video using FFmpeg
        CompileFramesToVideo(framesDir, "Day6Outputs/output.mp4");

        // Return the number of distinct positions visited
        return visitedPositions.Count.ToString();
    }

    /// <summary>
    /// Saves the current state of the map as an image using SkiaSharp.
    /// </summary>
    private void SaveFrame(List<string> map, int guardX, int guardY, char guardDirection, HashSet<(int x, int y)> visitedPositions, string filePath)
    {
        // Define cell size and image dimensions
        int cellSize = 20; // Size of each cell in pixels
        int width = map[0].Length * cellSize;
        int height = map.Count * cellSize;

        // Create a new SkiaSharp surface
        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;

            // Clear the canvas with a white background
            canvas.Clear(SKColors.White);

            // Draw the map
            for (int y = 0; y < map.Count; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    char cell = map[y][x];
                    SKColor color = SKColors.White; // Default color for empty cells

                    if (cell == '#')
                        color = SKColors.Black; // Obstacles
                    else if (visitedPositions.Contains((x, y)))
                        color = SKColors.LightGreen; // Visited positions
                    else if (x == guardX && y == guardY)
                        color = SKColors.Red; // Guard's current position

                    // Draw the cell
                    using (var paint = new SKPaint { Color = color })
                    {
                        canvas.DrawRect(x * cellSize, y * cellSize, cellSize, cellSize, paint);
                    }
                }
            }

            // Draw the guard's direction
            using (var paint = new SKPaint { Color = SKColors.Blue, TextSize = 12, IsAntialias = true })
            {
                canvas.DrawText(guardDirection.ToString(), guardX * cellSize, guardY * cellSize, paint);
            }

            // Save the frame as an image
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }
    }

    /// <summary>
    /// Compiles frames into a video using FFmpeg.
    /// </summary>
    private void CompileFramesToVideo(string framesDir, string outputVideoPath)
    {
        // Ensure the output directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(outputVideoPath));

        // Use FFmpeg to compile frames into a video
        string ffmpegCommand = $"ffmpeg -framerate 10 -i {Path.Combine(framesDir, "frame_%04d.png")} -c:v libx264 -r 30 -pix_fmt yuv420p {outputVideoPath}";
        System.Diagnostics.Process.Start("bash", $"-c \"{ffmpegCommand}\"").WaitForExit();
    }
}