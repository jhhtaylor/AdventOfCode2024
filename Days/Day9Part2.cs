// Days/Day9DiskFragmenterPart2.cs
using System;
using System.IO;
using System.Text;

public class Day9Part2 : IDay
{
    public string Solve()
    {
        // Read the disk map from input.
        // The disk map is a single line of digits.
        string filePath = "Inputs/Day9Input.txt";
        if (!File.Exists(filePath))
        {
            return "Error: Input file not found.";
        }
        
        string diskMap = File.ReadAllText(filePath).Trim();
        if (string.IsNullOrEmpty(diskMap))
            return "Error: Disk map is empty.";
        
        // Expand the dense disk map into an array of blocks.
        // The digits alternate between indicating a file length and a free-space length.
        // For each file block we output that many copies of the file's ID (starting at 0).
        // For each free-space block we output that many '.' characters.
        StringBuilder diskBuilder = new StringBuilder();
        int currentFileId = 0;
        for (int i = 0; i < diskMap.Length; i++)
        {
            int count = diskMap[i] - '0';
            if (i % 2 == 0)
            {
                // File block: append the current file ID (as a character) count times.
                diskBuilder.Append(new string((char)('0' + currentFileId), count));
                currentFileId++;
            }
            else
            {
                // Free space: append count dots.
                diskBuilder.Append(new string('.', count));
            }
        }
        char[] disk = diskBuilder.ToString().ToCharArray();
        // The total number of files is the number of file chunks we processed.
        int numFiles = currentFileId;
        
        // Process each file in order of decreasing file ID.
        // (Move each file at most once.)
        for (int fileId = numFiles - 1; fileId >= 0; fileId--)
        {
            // Find the contiguous segment (i.e. all adjacent blocks) for this file.
            // (After expansion the files are contiguous; later moves should keep them together.)
            int segStart = -1;
            int segLength = 0;
            for (int i = 0; i < disk.Length; i++)
            {
                if (disk[i] == (char)('0' + fileId))
                {
                    if (segStart == -1)
                    {
                        segStart = i;
                        segLength = 1;
                    }
                    else
                    {
                        segLength++;
                    }
                }
                else if (segStart != -1)
                {
                    // We found a contiguous segment; assume the file's blocks are all together.
                    break;
                }
            }
            // If no segment was found, skip (this should not happen).
            if (segStart == -1)
                continue;
            
            // Attempt to find a free contiguous span (of '.') entirely to the left of this file’s segment
            // that is long enough to accommodate the file (i.e. has length >= segLength).
            int targetPos = FindLeftmostFreeSegment(disk, segStart, segLength);
            if (targetPos != -1)
            {
                // "Move" the file: copy the file's character into the target region.
                for (int i = 0; i < segLength; i++)
                {
                    disk[targetPos + i] = (char)('0' + fileId);
                }
                // Clear the file's old location.
                for (int i = segStart; i < segStart + segLength; i++)
                {
                    disk[i] = '.';
                }
            }
        }
        
        // Compute the checksum:
        // For each file block (i.e. not '.'), add (position * fileID) to the checksum.
        long checksum = 0;
        for (int i = 0; i < disk.Length; i++)
        {
            if (disk[i] != '.')
            {
                int id = disk[i] - '0';
                checksum += i * id;
            }
        }
        
        return checksum.ToString();
    }
    
    /// <summary>
    /// Scans from the beginning of the disk up to (but not including) the given limit
    /// and returns the starting index of the leftmost contiguous free segment ('.')
    /// that is at least requiredLength in length.
    /// If no such segment exists, returns -1.
    /// </summary>
    private int FindLeftmostFreeSegment(char[] disk, int limit, int requiredLength)
    {
        int i = 0;
        while (i < limit)
        {
            if (disk[i] == '.')
            {
                int start = i;
                int count = 0;
                while (i < limit && disk[i] == '.')
                {
                    count++;
                    i++;
                }
                if (count >= requiredLength)
                {
                    return start;
                }
            }
            else
            {
                i++;
            }
        }
        return -1;
    }
}
