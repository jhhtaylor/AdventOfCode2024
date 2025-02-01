using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

public class Day6Part2Visualization : IDay
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

        // List to store valid obstruction positions
        List<(int x, int y)> validObstructionPositions = new List<(int x, int y)>();

        // Directory to save frames and videos
        string framesDir = "Day6Part2Frames";
        string outputsDir = "Day6Part2Outputs";
        Directory.CreateDirectory(framesDir);
        Directory.CreateDirectory(outputsDir);

        // List to store paths of generated videos
        List<string> videoPaths = new List<string>();

        // Counter for successful loop obstructions
        int obstructionNumber = 1;

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
                tempMap[y][x] = '#'; // New obstruction

                // Simulate the guard's movement and check for a loop
                if (IsLoopDetected(tempMap, guardX, guardY, directionIndex, framesDir, outputsDir, x, y, obstructionNumber))
                {
                    validObstructionPositions.Add((x, y));
                    videoPaths.Add(Path.Combine(outputsDir, $"obstruction_{x}_{y}.mp4"));
                    obstructionNumber++;
                }
            }
        }

        // Compile all videos into one
        if (videoPaths.Count > 0)
        {
            CompileAllVideos(videoPaths, Path.Combine(outputsDir, "final_compilation.mp4"));
        }

        // Return the number of valid obstruction positions
        return validObstructionPositions.Count.ToString();
    }

    /// <summary>
    /// Simulates the guard's movement and checks if a loop is detected.
    /// </summary>
    private bool IsLoopDetected(List<char[]> map, int startX, int startY, int startDirectionIndex, string framesDir, string outputsDir, int obstructionX, int obstructionY, int obstructionNumber)
    {
        // Set to track visited states (position + direction)
        HashSet<(int x, int y, int directionIndex)> visitedStates = new HashSet<(int x, int y, int directionIndex)>();

        // Set to track visited positions for visualization
        HashSet<(int x, int y)> visitedPositions = new HashSet<(int x, int y)>();

        // Variable to store the first revisited state (loop start)
        (int x, int y, int directionIndex)? loopStartState = null;

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

        // Directory to save frames for this obstruction
        string obstructionFramesDir = Path.Combine(framesDir, $"obstruction_{obstructionX}_{obstructionY}");
        Directory.CreateDirectory(obstructionFramesDir);

        // Frame counter
        int frameNumber = 0;

        // Simulate the guard's movement to detect the loop start
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

            // Check if the current state has been visited before
            var currentState = (guardX, guardY, directionIndex);
            if (visitedStates.Contains(currentState))
            {
                // If loopStartState is not set, this is the first revisited state
                if (loopStartState == null)
                {
                    loopStartState = currentState;
                    Console.WriteLine($"Loop start detected at: ({loopStartState.Value.x}, {loopStartState.Value.y})");
                }
                break; // Exit the simulation loop
            }

            // Add the current state to the set of visited states
            visitedStates.Add(currentState);
        }

        // Reset the guard's position and direction for the actual simulation
        guardX = startX;
        guardY = startY;
        directionIndex = startDirectionIndex;
        visitedStates.Clear();
        visitedPositions.Clear();

        // Simulate the guard's movement for visualization
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
            SaveFrame(map, guardX, guardY, directions[directionIndex].symbol, visitedPositions, obstructionX, obstructionY, obstructionNumber, loopStartState, Path.Combine(obstructionFramesDir, $"frame_{frameNumber:0000}.png"));
            frameNumber++;

            // Check if the current state has been visited before
            var currentState = (guardX, guardY, directionIndex);
            if (visitedStates.Contains(currentState))
            {
                // Compile frames into a video
                CompileFramesToVideo(obstructionFramesDir, Path.Combine(outputsDir, $"obstruction_{obstructionX}_{obstructionY}.mp4"));
                return true; // Loop detected
            }

            // Add the current state to the set of visited states
            visitedStates.Add(currentState);
        }

        return false; // No loop detected
    }

    /// <summary>
    /// Saves the current state of the map as an image using SkiaSharp.
    /// </summary>
    private void SaveFrame(List<char[]> map, int guardX, int guardY, char guardDirection, HashSet<(int x, int y)> visitedPositions, int obstructionX, int obstructionY, int obstructionNumber, (int x, int y, int directionIndex)? loopStartState, string filePath)
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
                    {
                        // Highlight the new obstruction in yellow
                        if (x == obstructionX && y == obstructionY)
                            color = SKColors.Yellow;
                        else
                            color = SKColors.Black; // Other obstacles
                    }
                    else if (visitedPositions.Contains((x, y)))
                        color = SKColors.LightGreen; // Visited positions
                    else if (x == guardX && y == guardY)
                        color = SKColors.Red; // Guard's current position

                    // Highlight the loop start position in magenta
                    if (loopStartState.HasValue && x == loopStartState.Value.x && y == loopStartState.Value.y)
                    {
                        color = SKColors.Magenta;
                    }

                    // Draw the cell
                    using (var paint = new SKPaint { Color = color })
                    {
                        canvas.DrawRect(x * cellSize, y * cellSize, cellSize, cellSize, paint);
                    }

                    // Draw the obstruction number on the yellow square
                    if (x == obstructionX && y == obstructionY)
                    {
                        using (var paint = new SKPaint { Color = SKColors.Black, TextSize = 12, IsAntialias = true, TextAlign = SKTextAlign.Center })
                        {
                            canvas.DrawText(obstructionNumber.ToString(), (x + 0.5f) * cellSize, (y + 0.75f) * cellSize, paint);
                        }
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
        // Use FFmpeg to compile frames into a video
        string ffmpegCommand = $"ffmpeg -framerate 10 -i {Path.Combine(framesDir, "frame_%04d.png")} -c:v libx264 -r 30 -pix_fmt yuv420p {outputVideoPath}";
        System.Diagnostics.Process.Start("bash", $"-c \"{ffmpegCommand}\"").WaitForExit();
    }

    /// <summary>
    /// Compiles all videos into a single video using FFmpeg.
    /// </summary>
    private void CompileAllVideos(List<string> videoPaths, string outputVideoPath)
    {
        // Create a text file listing all video paths
        string videosListPath = Path.Combine(Path.GetDirectoryName(outputVideoPath), "videos.txt");
        using (var writer = new StreamWriter(videosListPath))
        {
            foreach (var videoPath in videoPaths)
            {
                writer.WriteLine($"file '{Path.GetFullPath(videoPath)}'");
            }
        }

        // Use FFmpeg to concatenate all videos
        string ffmpegCommand = $"ffmpeg -f concat -safe 0 -i {videosListPath} -c copy {outputVideoPath}";
        System.Diagnostics.Process.Start("bash", $"-c \"{ffmpegCommand}\"").WaitForExit();

        // Clean up the videos list file
        File.Delete(videosListPath);
    }
}