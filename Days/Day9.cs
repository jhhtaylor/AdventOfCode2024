// Days/Day9DiskFragmenter.cs
using System;
using System.IO;
using System.Text;

public class Day9 : IDay
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
        // In the input digits alternate:
        //   file length, free space length, file length, free space, … 
        // For each file block we output that many copies of the file’s ID (starting at 0).
        // For each free space block we output that many '.' characters.
        StringBuilder diskBuilder = new StringBuilder();
        int fileId = 0;
        for (int i = 0; i < diskMap.Length; i++)
        {
            int count = diskMap[i] - '0';
            if (i % 2 == 0)
            {
                // File block: append the current file ID (as a digit) count times.
                diskBuilder.Append(new string((char)('0' + fileId), count));
                fileId++;
            }
            else
            {
                // Free space: append count dots.
                diskBuilder.Append(new string('.', count));
            }
        }
        char[] disk = diskBuilder.ToString().ToCharArray();
        
        // Uncomment the next line to see the expanded disk configuration.
        // Console.WriteLine("Initial Disk: " + new string(disk));

        // Perform the compaction:
        // Repeatedly move a block from the rightmost file block to the leftmost free space.
        // (That is, find the leftmost '.' and the rightmost non-'.'; then move the file block.)
        while (true)
        {
            int leftIndex = Array.IndexOf(disk, '.');
            int rightIndex = -1;
            for (int i = disk.Length - 1; i >= 0; i--)
            {
                if (disk[i] != '.')
                {
                    rightIndex = i;
                    break;
                }
            }
            
            // If there is no free block, or if the leftmost free block is after all file blocks, stop.
            if (leftIndex == -1 || leftIndex >= rightIndex)
                break;
            
            // Move the file block from the rightmost file cell to the leftmost free cell.
            disk[leftIndex] = disk[rightIndex];
            disk[rightIndex] = '.';
        }
        
        // Uncomment the next line to see the final disk configuration.
         //Console.WriteLine("Final Disk: " + new string(disk));

        // Compute the checksum:
        // For each file block (non '.'), add (position * fileID) to the checksum.
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
}
