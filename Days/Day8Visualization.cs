using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

public class Day8Visualization : IDay
{
    public string Solve()
    {
        string filePath = "Inputs/Day8Input.txt";

        if (!File.Exists(filePath))
        {
            return "Error: input.txt not found!";
        }

        var grid = File.ReadAllLines(filePath);

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

        string framesDir = "Day8Frames";
        string outputsDir = "Day8Outputs";
        Directory.CreateDirectory(framesDir);
        Directory.CreateDirectory(outputsDir);

        int frameNumber = 0;

        var antinodes = new HashSet<(int x, int y)>();
        for (int i = 0; i < antennas.Count; i++)
        {
            for (int j = i + 1; j < antennas.Count; j++)
            {
                if (antennas[i].frequency == antennas[j].frequency)
                {
                    var (x1, y1, _) = antennas[i];
                    var (x2, y2, _) = antennas[j];

                    var dx = x2 - x1;
                    var dy = y2 - y1;

                    var antinode1 = (x1 - dx, y1 - dy);
                    if (IsWithinBounds(antinode1, grid))
                    {
                        antinodes.Add(antinode1);
                        SaveFrame(grid, antennas, antinodes, Path.Combine(framesDir, $"frame_{frameNumber:0000}.png"));
                        frameNumber++;
                    }

                    var antinode2 = (x2 + dx, y2 + dy);
                    if (IsWithinBounds(antinode2, grid))
                    {
                        antinodes.Add(antinode2);
                        SaveFrame(grid, antennas, antinodes, Path.Combine(framesDir, $"frame_{frameNumber:0000}.png"));
                        frameNumber++;
                    }
                }
            }
        }

        CompileFramesToVideo(framesDir, Path.Combine(outputsDir, "output.mp4"));

        return antinodes.Count.ToString();
    }

    private bool IsWithinBounds((int x, int y) point, string[] grid)
    {
        return point.x >= 0 && point.x < grid[0].Length && point.y >= 0 && point.y < grid.Length;
    }

    private void SaveFrame(string[] grid, List<(int x, int y, char frequency)> antennas, HashSet<(int x, int y)> antinodes, string filePath)
    {
        int cellSize = 20;
        int width = grid[0].Length * cellSize;
        int height = grid.Length * cellSize;

        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

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

            using (var paint = new SKPaint { Color = SKColors.Black, TextSize = 12, IsAntialias = true })
            {
                foreach (var antenna in antennas)
                {
                    canvas.DrawText(antenna.frequency.ToString(), antenna.x * cellSize, antenna.y * cellSize, paint);
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
    private void CompileFramesToVideo(string framesDir, string outputVideoPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputVideoPath));

        string ffmpegCommand = $"ffmpeg -framerate 10 -i {Path.Combine(framesDir, "frame_%04d.png")} -c:v libx264 -r 30 -pix_fmt yuv420p {outputVideoPath}";
        System.Diagnostics.Process.Start("bash", $"-c \"{ffmpegCommand}\"").WaitForExit();
    }
}