// Days/Day8Part2Visualization.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

public class Day8Part2VisualizationSunflower : IDay
{
    public string Solve()
    {
        // Path to the input file (make sure to save the sample input below as "Inputs/Day8Input_Example.txt")
        string filePath = "Inputs/Day8Input_Sunflower.txt";
        if (!File.Exists(filePath))
        {
            return "Error: input.txt not found!";
        }

        // Read the input grid (each line must have the same length)
        var grid = File.ReadAllLines(filePath);

        // Parse the grid: every character that is not a '.' is considered an antenna.
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

        // Set up directories for output frames and final video.
        string framesDir = "Day8Part2Frames";
        string outputsDir = "Day8Part2Outputs";
        Directory.CreateDirectory(framesDir);
        Directory.CreateDirectory(outputsDir);

        int frameNumber = 0;
        var antinodes = new HashSet<(int x, int y)>();

        // Group antennas by frequency (all antennas in our sunflower are the letter 'S').
        var frequencyGroups = antennas.GroupBy(a => a.frequency);
        foreach (var group in frequencyGroups)
        {
            var antennasOfFrequency = group.ToList();
            if (antennasOfFrequency.Count < 2)
                continue; // At least two antennas are needed to create a line.
            
            for (int i = 0; i < antennasOfFrequency.Count; i++)
            {
                for (int j = i + 1; j < antennasOfFrequency.Count; j++)
                {
                    var (x1, y1, _) = antennasOfFrequency[i];
                    var (x2, y2, _) = antennasOfFrequency[j];

                    // Get every grid position (within bounds) on the infinite line
                    // defined by these two antennas using the reduced step approach.
                    var linePositions = GetExtendedLinePositions(x1, y1, x2, y2, grid);
                    //Console.WriteLine($"Line between ({x1}, {y1}) and ({x2}, {y2}):");
                    foreach (var pos in linePositions)
                    {
                        //Console.WriteLine($"  ({pos.x}, {pos.y})");
                        // Only when a new antinode is discovered do we save a frame.
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
        // Compile all frames into a video (requires ffmpeg installed).
        CompileFramesToVideo(framesDir, Path.Combine(outputsDir, "output.mp4"));
        return antinodes.Count.ToString();
    }

    /// <summary>
    /// Returns all grid positions (within the grid bounds) that lie on the infinite line
    /// defined by the two antennas at (x1,y1) and (x2,y2).
    /// </summary>
    private List<(int x, int y)> GetExtendedLinePositions(int x1, int y1, int x2, int y2, string[] grid)
    {
        var positions = new List<(int x, int y)>();

        int dx = x2 - x1;
        int dy = y2 - y1;
        int g = GCD(Math.Abs(dx), Math.Abs(dy));
        if (g == 0)
        {
            if (IsWithinBounds((x1, y1), grid))
                positions.Add((x1, y1));
            return positions;
        }

        int stepX = dx / g;
        int stepY = dy / g;

        // Back up from (x1,y1) until we leave the grid.
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

        // Now step forward from the starting point until we exit the grid.
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
    /// Computes the greatest common divisor using Euclid's algorithm.
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
    /// Returns true if the given point lies within the bounds of the grid.
    /// </summary>
    private bool IsWithinBounds((int x, int y) point, string[] grid)
    {
        return point.x >= 0 && point.x < grid[0].Length && point.y >= 0 && point.y < grid.Length;
    }

    /// <summary>
    /// Draws and saves a frame as a PNG image to visualize the current grid.
    /// Color scheme for a sunflower:
    /// - Background: LightBlue  
    /// - Antenna cells (petals): Gold  
    /// - Antinode cells (empty): Orange  
    /// - Overlap (antenna and antinode): Red  
    /// - Antenna labels are drawn in DarkGreen.
    /// </summary>
    private void SaveFrame(string[] grid, List<(int x, int y, char frequency)> antennas, HashSet<(int x, int y)> antinodes, string filePath)
    {
        int cellSize = 20;
        int width = grid[0].Length * cellSize;
        int height = grid.Length * cellSize;

        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.LightBlue);

            for (int y = 0; y < grid.Length; y++)
            {
                for (int x = 0; x < grid[y].Length; x++)
                {
                    char cell = grid[y][x];
                    SKColor color = SKColors.LightBlue; // default: background

                    if (cell != '.')
                        color = SKColors.Gold; // antenna (petal)
                    else if (antinodes.Contains((x, y)))
                        color = SKColors.Orange; // antinode in an empty cell

                    // If an antenna is also an antinode, use red.
                    if (antinodes.Contains((x, y)) && cell != '.')
                        color = SKColors.Red;

                    using (var paint = new SKPaint { Color = color })
                    {
                        canvas.DrawRect(x * cellSize, y * cellSize, cellSize, cellSize, paint);
                    }
                }
            }

            using (var paint = new SKPaint { Color = SKColors.DarkGreen, TextSize = 12, IsAntialias = true })
            {
                foreach (var antenna in antennas)
                {
                    canvas.DrawText(antenna.frequency.ToString(), antenna.x * cellSize, antenna.y * cellSize + cellSize, paint);
                }
            }

            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }
    }

    /// <summary>
    /// Compiles all the saved PNG frames into a video using ffmpeg.
    /// (Make sure that ffmpeg is installed and available on your system.)
    /// </summary>
    private void CompileFramesToVideo(string framesDir, string outputVideoPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputVideoPath));
        string ffmpegCommand = $"ffmpeg -framerate 10 -i {Path.Combine(framesDir, "frame_%04d.png")} -c:v libx264 -r 30 -pix_fmt yuv420p {outputVideoPath}";
        System.Diagnostics.Process.Start("bash", $"-c \"{ffmpegCommand}\"").WaitForExit();
    }
}
