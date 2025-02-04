// Days/Day8Part2Visualization.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

public class Day8Part2Visualization : IDay
{
    public string Solve()
    {
        // Path to the input file (example input)
        string filePath = "Inputs/Day8Input.txt";

        if (!File.Exists(filePath))
        {
            return "Error: input.txt not found!";
        }

        // Read the input grid
        var grid = File.ReadAllLines(filePath);

        // Parse the grid to find antennas.
        // Every character that is not a '.' is an antenna.
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

        // Set up directories for the output frames and video
        string framesDir = "Day8Part2Frames";
        string outputsDir = "Day8Part2Outputs";
        Directory.CreateDirectory(framesDir);
        Directory.CreateDirectory(outputsDir);

        int frameNumber = 0;
        var antinodes = new HashSet<(int x, int y)>();

        // Group antennas by frequency since only antennas of the same frequency produce antinodes.
        var frequencyGroups = antennas.GroupBy(a => a.frequency);

        // For every pair of antennas of the same frequency, compute the full extended line.
        foreach (var group in frequencyGroups)
        {
            var antennasOfFrequency = group.ToList();
            if (antennasOfFrequency.Count < 2)
                continue; // No pair exists so skip

            for (int i = 0; i < antennasOfFrequency.Count; i++)
            {
                for (int j = i + 1; j < antennasOfFrequency.Count; j++)
                {
                    var (x1, y1, _) = antennasOfFrequency[i];
                    var (x2, y2, _) = antennasOfFrequency[j];

                    // Get every grid position on the infinite line (within grid bounds)
                    // defined by these two antennas.
                    var linePositions = GetExtendedLinePositions(x1, y1, x2, y2, grid);
                    //Console.WriteLine($"Line between ({x1}, {y1}) and ({x2}, {y2}):");
                    foreach (var pos in linePositions)
                    {
                        //Console.WriteLine($"  ({pos.x}, {pos.y})");
                        // Only update (and save a frame) if we add a new antinode.
                        if (antinodes.Add(pos))
                        {
                            SaveFrame(grid, antennas, antinodes, Path.Combine(framesDir, $"frame_{frameNumber:0000}.png"));
                            frameNumber++;
                        }
                    }
                }
            }
        }

        //Console.WriteLine($"Total antinodes: {antinodes.Count}");
        // Compile the frames into a video (requires ffmpeg)
        CompileFramesToVideo(framesDir, Path.Combine(outputsDir, "output.mp4"));
        return antinodes.Count.ToString();
    }

    /// <summary>
    /// Returns all grid positions (within the grid bounds) that lie on the infinite line defined by (x1, y1) and (x2, y2).
    /// </summary>
    private List<(int x, int y)> GetExtendedLinePositions(int x1, int y1, int x2, int y2, string[] grid)
    {
        var positions = new List<(int x, int y)>();

        int dx = x2 - x1;
        int dy = y2 - y1;
        int g = GCD(Math.Abs(dx), Math.Abs(dy));

        // If the antennas are at the same position, simply return that point.
        if (g == 0)
        {
            if (IsWithinBounds((x1, y1), grid))
                positions.Add((x1, y1));
            return positions;
        }

        // Determine the smallest (primitive) step.
        int stepX = dx / g;
        int stepY = dy / g;

        // Step backward from one antenna until we exit the grid to find the start.
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

        // Now, step forward (using the primitive step) until we're out of bounds.
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
    /// Computes the greatest common divisor of two integers using Euclid's algorithm.
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
    /// Checks if a grid position is within the bounds of the grid.
    /// </summary>
    private bool IsWithinBounds((int x, int y) point, string[] grid)
    {
        return point.x >= 0 && point.x < grid[0].Length && point.y >= 0 && point.y < grid.Length;
    }

    /// <summary>
    /// Saves a frame as a PNG image that visualizes the current state of the grid.
    /// Antennas are drawn in blue (or purple if they also have an antinode),
    /// and antinodes (that are not antennas) are drawn in red.
    /// </summary>
    private void SaveFrame(string[] grid, List<(int x, int y, char frequency)> antennas, HashSet<(int x, int y)> antinodes, string filePath)
    {
        int cellSize = 20;
        int width = grid[0].Length * cellSize;
        int height = grid.Length * cellSize;

        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            // Draw the grid cells.
            for (int y = 0; y < grid.Length; y++)
            {
                for (int x = 0; x < grid[y].Length; x++)
                {
                    char cell = grid[y][x];
                    SKColor color = SKColors.White;

                    if (cell != '.')
                        color = SKColors.Blue;
                    else if (antinodes.Contains((x, y)))
                        color = SKColors.Red;

                    // If a cell is both an antenna and an antinode, use purple.
                    if (antinodes.Contains((x, y)) && cell != '.')
                    {
                        color = SKColors.Purple;
                    }

                    using (var paint = new SKPaint { Color = color })
                    {
                        canvas.DrawRect(x * cellSize, y * cellSize, cellSize, cellSize, paint);
                    }
                }
            }

            // Draw the antenna characters.
            using (var paint = new SKPaint { Color = SKColors.Black, TextSize = 12, IsAntialias = true })
            {
                foreach (var antenna in antennas)
                {
                    // Offset the text slightly for better centering.
                    canvas.DrawText(antenna.frequency.ToString(), antenna.x * cellSize, antenna.y * cellSize + cellSize, paint);
                }
            }

            // Save the frame as a PNG.
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }
    }

    /// <summary>
    /// Uses ffmpeg to compile all frame images into a video.
    /// Make sure ffmpeg is installed and available in your system path.
    /// </summary>
    private void CompileFramesToVideo(string framesDir, string outputVideoPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputVideoPath));

        // Build the ffmpeg command.
        string ffmpegCommand = $"ffmpeg -framerate 10 -i {Path.Combine(framesDir, "frame_%04d.png")} -c:v libx264 -r 30 -pix_fmt yuv420p {outputVideoPath}";
        System.Diagnostics.Process.Start("bash", $"-c \"{ffmpegCommand}\"").WaitForExit();
    }
}
