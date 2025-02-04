// Days/Day10HoofItVisualization.cs
using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;

public class Day10Visualization : IDay
{
    // Grid dimensions and data.
    private int rows, cols;
    private int[,] grid;
    // A list to hold all discovered trails. Each trail is a list of (row, col) coordinates.
    private List<List<(int r, int c)>> allTrails = new List<List<(int, int)>>();

    // Direction arrays for DFS (up, down, left, right).
    private readonly int[] dr = new int[] { -1, 1, 0, 0 };
    private readonly int[] dc = new int[] { 0, 0, -1, 1 };

    public string Solve()
    {
        // Read the topographic map from input.
        // Each line is a row; each character is a digit (0–9).
        string filePath = "Inputs/Day10Input_Example.txt";
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
                return "Error: Inconsistent row lengths.";
            for (int c = 0; c < cols; c++)
            {
                grid[r, c] = lines[r][c] - '0';
            }
        }
        
        // For every trailhead (cell with height 0) start a DFS to collect trails.
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] == 0)
                {
                    List<(int r, int c)> path = new List<(int, int)>();
                    DFS(r, c, path);
                }
            }
        }
        
        // Visualize the grid and overlay all trails.
        VisualizeTrails();
        
        return $"Trails visualized: {allTrails.Count} total trails.";
    }
    
    /// <summary>
    /// Performs a DFS from cell (r, c) following only moves that increase the height by exactly 1.
    /// When a cell of height 9 is reached, the complete path is saved.
    /// </summary>
    private void DFS(int r, int c, List<(int r, int c)> currentPath)
    {
        currentPath.Add((r, c));
        if (grid[r, c] == 9)
        {
            allTrails.Add(new List<(int, int)>(currentPath));
            currentPath.RemoveAt(currentPath.Count - 1);
            return;
        }
        
        int nextHeight = grid[r, c] + 1;
        for (int i = 0; i < 4; i++)
        {
            int nr = r + dr[i];
            int nc = c + dc[i];
            if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
            {
                if (grid[nr, nc] == nextHeight)
                {
                    DFS(nr, nc, currentPath);
                }
            }
        }
        currentPath.RemoveAt(currentPath.Count - 1);
    }
    
    /// <summary>
    /// Visualizes the topographic map with overlaid trails.
    /// Each cell is drawn with its height (the number is drawn in the top‑left corner using a small font),
    /// and each trail is drawn with arrow markers (using a larger font) positioned away from the center
    /// based on the move’s direction.
    /// Each trail is rendered in a distinct color.
    /// </summary>
    private void VisualizeTrails()
    {
        // Define the cell size and image dimensions.
        int cellSize = 40;
        int width = cols * cellSize;
        int height = rows * cellSize;
        
        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;
            // Clear the background.
            canvas.Clear(SKColors.White);
            
            // Draw grid cells (with borders) and the cell's height in the top-left.
            using (var borderPaint = new SKPaint { Color = SKColors.LightGray, IsStroke = true, StrokeWidth = 1 })
            using (var numberPaint = new SKPaint { Color = SKColors.Black, TextSize = cellSize * 0.3f, IsAntialias = true })
            {
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        var rect = new SKRect(c * cellSize, r * cellSize, (c + 1) * cellSize, (r + 1) * cellSize);
                        canvas.DrawRect(rect, borderPaint);
                        
                        // Draw the cell's height number in the top-left corner with a small margin.
                        string heightStr = grid[r, c].ToString();
                        canvas.DrawText(heightStr, c * cellSize + 4, r * cellSize + numberPaint.TextSize + 2, numberPaint);
                    }
                }
            }
            
            // Define an array of colors to cycle through for the different trails.
            SKColor[] trailColors = new SKColor[] {
                SKColors.Red, SKColors.Blue, SKColors.Green, SKColors.Orange,
                SKColors.Purple, SKColors.Teal, SKColors.Magenta, SKColors.Brown,
                SKColors.Cyan, SKColors.DarkRed
            };
            
            // Prepare a paint for drawing arrow symbols.
            using (var arrowPaint = new SKPaint { IsAntialias = true, TextSize = cellSize * 0.6f, Typeface = SKTypeface.FromFamilyName("Arial") })
            {
                // For each trail, draw arrow markers for each step.
                for (int t = 0; t < allTrails.Count; t++)
                {
                    List<(int r, int c)> trail = allTrails[t];
                    arrowPaint.Color = trailColors[t % trailColors.Length];
                    
                    // For each adjacent pair of points in the trail, determine the arrow direction.
                    for (int i = 0; i < trail.Count - 1; i++)
                    {
                        var (r1, c1) = trail[i];
                        var (r2, c2) = trail[i + 1];
                        string arrow = GetArrowSymbol(r1, c1, r2, c2);
                        
                        // Compute the base center of the starting cell.
                        float baseCenterX = c1 * cellSize + cellSize / 2;
                        float baseCenterY = r1 * cellSize + cellSize / 2;
                        // Define an offset (quarter of the cell size) to separate the arrow from the center.
                        float offset = cellSize / 4f;
                        float arrowX = baseCenterX;
                        float arrowY = baseCenterY;
                        
                        // Position the arrow based on its direction.
                        // (This offset ensures the arrow doesn't overlap the small number in the top-left.)
                        switch (arrow)
                        {
                            case "^": arrowY = baseCenterY - offset; break;
                            case "v": arrowY = baseCenterY + offset; break;
                            case ">": arrowX = baseCenterX + offset; break;
                            case "<": arrowX = baseCenterX - offset; break;
                        }
                        
                        // Draw the arrow symbol.
                        SKRect arrowBounds = new SKRect();
                        arrowPaint.MeasureText(arrow, ref arrowBounds);
                        // Adjust to center the arrow text at the computed position.
                        float drawX = arrowX - arrowBounds.MidX;
                        float drawY = arrowY - arrowBounds.MidY;
                        canvas.DrawText(arrow, drawX, drawY, arrowPaint);
                    }
                }
            }
            
            // Save the resulting image.
            string outputDir = "Day10Outputs";
            Directory.CreateDirectory(outputDir);
            string outputPath = System.IO.Path.Combine(outputDir, "Day10Trails.png");
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(outputPath))
            {
                data.SaveTo(stream);
            }
        }
    }
    
    /// <summary>
    /// Returns an arrow symbol ("^", "v", ">", "<") representing the move from (r1, c1) to (r2, c2).
    /// </summary>
    private string GetArrowSymbol(int r1, int c1, int r2, int c2)
    {
        if (r2 == r1 && c2 == c1 + 1)
            return ">";
        if (r2 == r1 && c2 == c1 - 1)
            return "<";
        if (r2 == r1 - 1 && c2 == c1)
            return "^";
        if (r2 == r1 + 1 && c2 == c1)
            return "v";
        return "?";
    }
}
